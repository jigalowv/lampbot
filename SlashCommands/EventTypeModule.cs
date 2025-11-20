using System.Text;
using Discord.Interactions;
using lampbot.Data;
using lampbot.Entities;

namespace lampbot.SlashCommands
{
    [Group("types", "basic event types' operations.")]
    public class EventTypeModule : ModuleBase
    {
        public EventTypeModule(DataContext context)
            :base(context)
        { }

        [SlashCommand("add", "adds new event type.")]
        public async Task AddAsync([Summary("name", "event type's name")]string name)
        {
            if (!await EnsureExecutorHasAccessAsync(await GetExecutorAsync(), Role.Lead))
                return;

            name = name.ToLower();

            var eventType = _context.EventTypes
                .Where(et => et.Name == name)
                .FirstOrDefault();

            if (eventType is not null)
            {
                await RespondAsync("'{name}' already exists", ephemeral: true);
                return;
            }

            await _context.EventTypes.AddAsync(new EventType() { Name = name });
            await _context.SaveChangesAsync();
            await RespondAsync("added successfully.", ephemeral: true);
        }

        [SlashCommand("remove", "removes the event type.")]
        public async Task RemoveAsync([Summary("name", "event type's name")]string name)
        {
            if (!await EnsureExecutorHasAccessAsync(await GetExecutorAsync(), Role.Lead))
                return;

            var eventType = await FindEventType(name);

            if (!await EnsureObjectExistsAsync(eventType, "event_type")) 
                return;

            _context.EventTypes.Remove(eventType!);
            await _context.SaveChangesAsync();
            await RespondAsync($"'{eventType}' removed successfully", ephemeral: true);
        }
        
        [SlashCommand("show", "shows all events.")]
        public async Task ShowAsync()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Event Types:");

            foreach (var et in _context.EventTypes.OrderBy(et => et.Name))
            {
                sb.AppendLine("- " + et.Name);
            }

            await RespondAsync(sb.ToString(), ephemeral: true);
        }
    }
}