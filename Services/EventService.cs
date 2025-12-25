using System.Globalization;
using lampbot.Data;
using lampbot.Entities;
using lampbot.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace lampbot.Services
{
    public class EventService : ServiceBase
    {
        public EventService(
            DataContext context, 
            IConfiguration config)
            : base(context, config)
        { }

        public async Task<string> AddAsync(
            ulong executorId,
            string name,
            string startDateStr,
            string startTimeStr,
            string? endDtStr
        )
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), 
                    $"<@{executorId}>");

            if (executor.Role < Role.Lead)
                return _config.GetResponse(Constants.NoAccess, nameof(Role), Constants.Access);
                
            bool isValid = false;

            isValid = DateOnly.TryParseExact(startDateStr, Constants.DateFormat, 
                CultureInfo.InvariantCulture, DateTimeStyles.None, 
                out DateOnly startDate);

            if (!isValid)
                return _config.GetResponse(Constants.Invalid, Constants.Format, 
                    $"'{startDateStr}'");

            isValid = TimeOnly.TryParseExact(startTimeStr, Constants.TimeFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None,
                out TimeOnly startTime);

            if(!isValid)
                return _config.GetResponse(Constants.Invalid, Constants.Format, 
                    $"'{startTimeStr}'");

            DateTime? endDt = null;
            if (endDtStr is not null)
            {
                isValid = DateTime.TryParseExact(endDtStr, Constants.DateTimeFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime result);
                endDt = result;
            }

            if(!isValid)
                return _config.GetResponse(Constants.Invalid, Constants.Format, 
                    $"'{endDtStr}'");

            var eventType = await _context.EventTypes
                .Where(i => i.Name == name)
                .FirstOrDefaultAsync();

            if (eventType is null)
                return _config.GetResponse(Constants.NotFound, nameof(EventType), $"'{name}'");

            await _context.Events.AddAsync(new Event()
            {
                StartDate = startDate,
                StartTime = startTime,
                EndDt = endDt,
                EventTypeId = eventType.Id
            });

            await _context.SaveChangesAsync();

            return _config.GetResponse(Constants.Add, nameof(Event), $"'{name}'");
        }
    
        public async Task<string> RemoveAsync(
            ulong executorId,
            string startDateStr,
            string name
        )
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), $"<@{executorId}>");

            if (executor.Role < Role.Lead)
                return _config.GetResponse(Constants.NoAccess, nameof(Role), Constants.Access);

            bool isValid = false;

            isValid = DateOnly.TryParseExact(startDateStr, Constants.DateFormat, 
                CultureInfo.InvariantCulture, DateTimeStyles.None, 
                out DateOnly startDate);

            if (!isValid)
                return _config.GetResponse(Constants.Invalid, Constants.Format, 
                    $"'{startDateStr}'");

            var ev = await _context.Events
                .Include(i => i.EventType)
                .Where(i => i.StartDate == startDate && i.EventType!.Name == name)
                .FirstOrDefaultAsync();
            
            if (ev is null)
                return _config.GetResponse(Constants.NotFound, nameof(Event), 
                    $"({startDate}: '{name}')");

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return _config.GetResponse(Constants.Remove, nameof(Event), $"({startDate}: '{name}')");
        }
    }
}