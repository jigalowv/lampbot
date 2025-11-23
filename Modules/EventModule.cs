using System.Globalization;
using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using lampbot.Data;
using lampbot.Entities;
using lampbot.Services;

namespace lampbot.SlashCommands
{
    [Group("events", "all events operations.")]
    public sealed class EventModule : ModuleBase
    {
        private readonly EventService _service;

        public EventModule(EventService service)
        {
            _service = service;    
        }

        [SlashCommand("add", "adds new event to the database.")]
        public async Task AddAsync(
            [Summary("event_type", "type of the event.")]string name, 
            [Summary("start_date", "start date; format: 'dd.MM.yy'.")]string startDateStr,
            [Summary("start_time", "start time; format: 'HH.mm'.")]string startTimeStr, 
            [Summary("end_dt", "end datetime; format: 'dd.MM.yy HH:mm'.")]string? endDtStr = null)
        {
            var response = await _service.AddAsync(Context.User.Id, name.ToLower(), startDateStr, 
                startTimeStr, endDtStr);

            await RespondAsync(response, ephemeral: true);
        }

        // [SlashCommand("add-user", "adds a fact of the user's participation.")]
        // public async Task AddUserAsync(
        //     [Summary("user", "user to add.")] SocketUser user,
        //     [Summary("start_date", "start date; format: 'dd.MM.yy'.")] string? startDateStr = null,
        //     [Summary("is_active", "was user been active?")] bool isActive = false)

        [SlashCommand("remove", "removes an event from the database.")]
        public async Task RemoveAsync(
            [Summary("start_dt", "start datetime; format: 'dd.MM.yy'.")]
            string startDateStr,
            
            [Summary("event_type", "type of the event.")]
            string name)
        {
            var response = await _service.RemoveAsync(Context.User.Id, startDateStr, name);

            await RespondAsync(response, ephemeral: true);
        }

        // [SlashCommand("set-time", "sets the event's time properties.")]
        // public async Task SetTimeAsync(
        //     [Summary("start_date", "start date; format: 'dd.MM.yy'.")] 
        //     string? startDateStr = null,
            
        //     [Summary("new_start_dt", "new start datetime; format: 'dd.MM.yy HH:mm'.")] 
        //     string? newStartDtStr = null,
            
        //     [Summary("new_end_dt", "new end datetime; format: 'dd.MM.yy HH:mm'.")] 
        //     string? newEndDtStr = null)

        // [SlashCommand("set-type", "sets the event's type property")]
        // public async Task SetTypeAsync(
        //     [Summary("new_event_type", "the event's new type.")] string type,
        //     [Summary("start_date", "start date; format: 'dd.MM.yy'.")] string? startDateStr = null,
        //     [Summary("index", "index if several events exist on this date")] int? index = null)
    }
}