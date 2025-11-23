using Discord.Interactions;
using lampbot.Services;

namespace lampbot.SlashCommands
{
    [Group("types", "basic event types' operations.")]
    public class EventTypeModule : ModuleBase
    {
        private readonly EventTypeService _service;

        public EventTypeModule(EventTypeService service)
        {
            _service = service;
        }

        [SlashCommand("add", "adds new event type.")]
        public async Task AddAsync([Summary("name", "event type's name")]string name)
        {
            var response = await _service.AddAsync(Context.User.Id, name.ToLower());
            
            await RespondAsync(response, ephemeral: true);
        }

        [SlashCommand("remove", "removes the event type.")]
        public async Task RemoveAsync([Summary("name", "event type's name")]string name)
        {
            var response = await _service.RemoveAsync(Context.User.Id, name.ToLower());
            
            await RespondAsync(response, ephemeral: true);
        }
        
        [SlashCommand("show", "shows all events.")]
        public async Task ShowAsync()
        {
            var response = _service.ShowAsync();
            
            await RespondAsync(response, ephemeral: true);
        }
    }
}