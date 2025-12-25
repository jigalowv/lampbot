using System.Text;
using lampbot.Data;
using lampbot.Entities;
using lampbot.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace lampbot.Services
{
    public class EventTypeService : ServiceBase
    {
        public EventTypeService(
            DataContext context, 
            IConfiguration config) 
            : base(context, config)
        { }

        public async Task<string> AddAsync(
            ulong executorId,
            string name
        )
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), $"<@{executorId}>");

            if (executor.Role < Role.Curator)
                return _config.GetResponse(Constants.NoAccess, nameof(Role), $"access");

            var isNotNull = await _context.EventTypes
                .Where(i => i.Name == name)
                .FirstOrDefaultAsync() is not null;

            if (isNotNull)
                return _config.GetResponse(Constants.AlreadyExists, nameof(EventType), $"'{name}'");
            
            await _context.EventTypes.AddAsync(new EventType()
            {
               Name = name 
            });

            await _context.SaveChangesAsync();

            return _config.GetResponse(Constants.Add, nameof(EventType), $"'{name}'");
        }

        public async Task<string> RemoveAsync(
            ulong executorId,
            string name
        )
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), $"<@{executorId}>");

            if (executor.Role < Role.Curator)
                return _config.GetResponse(Constants.NoAccess, nameof(Role), $"access");

            var eventType = await _context.EventTypes
                .Where(i => i.Name == name)
                .FirstOrDefaultAsync();

            if (eventType is null)
                return _config.GetResponse(Constants.NotFound, nameof(EventType), $"'{name}'");

            _context.Remove(eventType);
            await _context.SaveChangesAsync();

            return _config.GetResponse(Constants.Remove, nameof(EventType), $"'{name}'");
        }

        public string ShowAsync()
        {
            var sb = new StringBuilder()
                .AppendLine("Event types:```");

            foreach (var eventType in _context.EventTypes)
            {
                sb.AppendLine()
                    .Append("+ ")
                    .Append(eventType.Name);
            }

            sb.AppendLine("```");

            return sb.ToString();
        }
    }
}