using System.Globalization;
using System.Text;
using Discord;
using Discord.Interactions;
using lampbot.Data;
using lampbot.Entities;

namespace lampbot.SlashCommands
{
    [Group("events", "all events operations.")]
    public class EventModule : ModuleBase
    {
        private const string _dtFormat = "dd.MM.yy HH:mm";
        private const string _dateOnlyFormat = "dd.MM.yy";

        public EventModule(DataContext context) 
            : base(context)
        { }

        [SlashCommand("add", "adds new event to the database.")]
        public async Task AddAsync(
            [Summary("event_type", "type of the event.")]string eventTypeStr, 
            [Summary("start_dt", "start datetime; format: 'dd.MM.yy HH:mm'.")]string startDtStr, 
            [Summary("end_dt", "end datetime; format: 'dd.MM.yy HH:mm'.")]string? endDtStr = null)
        {
            if (!await EnsureExecutorHasAccessAsync(await GetExecutorAsync(), Role.User))
                return;

            var eventType = await FindEventType(eventTypeStr.ToLower());
            
            if (!await EnsureObjectExistsAsync(eventType, "event type"))
                return;

            var (startOk, startDt) = await EnsureDateTimeHasValidFormatAsync(startDtStr); 
            if (!startOk) return;

            DateTime? endDt = null;
            if (endDtStr is not null)
            {
                (var endOk, endDt) = await EnsureDateTimeHasValidFormatAsync(endDtStr); 
                if (!endOk) return;
            }

            await _context.Events.AddAsync(new Event()
            {
                EventTypeId = eventType!.Id,
                StartDt = startDt,
                EndDt = endDt
            });

            await _context.SaveChangesAsync();
            await RespondAsync("added successfully.", ephemeral: true);
        }

        [SlashCommand("remove", "removes an event from the database.")]
        public async Task RemoveAsync(
            [Summary("start_dt", "start datetime; format: 'dd.MM.yy'.")]string startDateStr, 
            [Summary("index", "")]int? index = null)
        {
            if (!await EnsureExecutorHasAccessAsync(await GetExecutorAsync(), Role.User))
                return;

            var (ok, date) = await EnsureDateOnlyHasValidFormatAsync(startDateStr); 
            if (!ok) return;

            (ok, var ev) = await TryGetEvent(date, index);
            if (!ok) return;

            _context.Remove(ev!);
            await _context.SaveChangesAsync();
            await RespondAsync("removed successfully.", ephemeral: true);
        }

        [SlashCommand("set-time", "sets the event's time properties.")]
        public async Task SetTimeAsync(
            [Summary("start_date", "start date; format: 'dd.MM.yy'.")] string? startDateStr = null,
            [Summary("new_start_dt", "new start datetime; format: 'dd.MM.yy'.")] string? newStartDtStr = null,
            [Summary("new_end_dt", "new end datetime; format: 'dd.MM.yy'.")] string? newEndDtStr = null,
            [Summary("index", "index if several events exist on this date")] int? index = null)
        {
            if (newStartDtStr is null && newEndDtStr is null)
            {
                await RespondAsync("no arguments.", ephemeral: true);
                return;
            }

            if (!await EnsureExecutorHasAccessAsync(await GetExecutorAsync(), Role.User))
                return;

            DateOnly? date = null;
            if (startDateStr is not null)
            {
                (var ok, date) = await EnsureDateOnlyHasValidFormatAsync(startDateStr);
                if (!ok) return;
            }

            var (dateOk, ev) = date is not null 
                ? await TryGetEvent(date.Value, index) 
                : await TryGetLastEventAsync();

            if (!dateOk) return;

            if (newStartDtStr is not null)
            {
                (var ok, ev!.StartDt) = await EnsureDateTimeHasValidFormatAsync(newStartDtStr);
                if (!ok) return;
            }

            if (newEndDtStr is not null)
            {
                (var ok, ev!.EndDt) = await EnsureDateTimeHasValidFormatAsync(newEndDtStr);
                if (!ok) return;
            }

            await _context.SaveChangesAsync();
            await RespondAsync("datetime updated successfully.", ephemeral: true);
        }

        [SlashCommand("set-type", "sets the event's type property")]
        public async Task SetTypeAsync(
            [Summary("new_event_type", "the event's new type.")] string type,
            [Summary("start_date", "start date; format: 'dd.MM.yy'.")] string? startDateStr = null,
            [Summary("index", "index if several events exist on this date")] int? index = null)
        {
            if (!await EnsureExecutorHasAccessAsync(await GetExecutorAsync(), Role.User))
                return;

            DateOnly? date = null;
            if (startDateStr is not null)
            {
                (var ok, date) = await EnsureDateOnlyHasValidFormatAsync(startDateStr);
                if (!ok) return;
            }

            var eventType = await FindEventType(type.ToLower());
            
            if (!await EnsureObjectExistsAsync(eventType, "event type"))
                return;

            var (dateOk, ev) = date is not null 
                ? await TryGetEvent(date.Value, index) 
                : await TryGetLastEventAsync();

            if (!dateOk) return;

            ev!.EventTypeId = eventType!.Id;
            await _context.SaveChangesAsync();
            await RespondAsync("event type updated successfully.", ephemeral: true);
        }

        private async Task PrintEventsAsync(List<Event> events)
        {
            var eb = new EmbedBuilder()
                    .WithFooter(events
                        .FirstOrDefault()!.StartDt.Date
                        .ToString(_dateOnlyFormat));
                
            var sb = new StringBuilder()
                .AppendLine("```");
            for (int i = 0; i < events.Count; i++)
            {
                sb.AppendLine($"index: {i + 1:00}; name: {events[i].EventType!.Name}");
            }
            sb.AppendLine("```");

            eb.WithDescription(sb.ToString());
            await RespondAsync(embed: eb.Build(), ephemeral: true);
        }

        private async Task<(bool ok, Event? ev)> TryGetEvent(DateOnly date, int? index)
        {
            var events = FindEventsByDate(date);

            if (events.Count == 0)
            {
                await RespondAsync("the event does not exist.", ephemeral: true);
                return (false, null);
            }

            if (events.Count > 1)
            {
                if (index is null || index < 1 || index > events.Count)
                {
                    await PrintEventsAsync(events);
                    return (false, null);
                }

                return (true, events[(int)index - 1]);
            }

            return (true, events[0]);
        }

        private async Task<(bool isValid, DateTime dt)> EnsureDateTimeHasValidFormatAsync(string dtStr)
        {
            bool isValid = DateTime.TryParseExact(dtStr, _dtFormat, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, 
                out DateTime dt);

            if (!isValid)
            {
                await RespondAsync("the datetime string has wrong format.");
                return (false, DateTime.MinValue);
            }

            return (true, dt);
        }

        private async Task<(bool isValid, DateOnly date)> EnsureDateOnlyHasValidFormatAsync(string dateStr)
        {
            bool isValid = DateOnly.TryParseExact(
                dateStr,
                _dateOnlyFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateOnly date
            );

            if (!isValid)
            {
                await RespondAsync("the date string has wrong format.");
                return (false, DateOnly.MinValue);
            }

            return (true, date);
        }
    }
}