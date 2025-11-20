using System.Globalization;
using Discord;
using Discord.Interactions;
using lampbot.Data;
using lampbot.Entities;
using Microsoft.EntityFrameworkCore;

namespace lampbot.SlashCommands
{
    public class ModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        protected readonly DataContext _context;

        public ModuleBase(DataContext context)
        {
            _context = context;
        }

        protected async Task<(bool, Event?)> TryGetLastEventAsync()
        { 
            var ev = await _context.Events
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            
            if (ev is null)
            {
                await RespondAsync("no events in the database.");
                return (false, null);
            }

            return (true, ev);
        }

        protected Task<User?> GetExecutorAsync()
            => _context.Users.FindAsync(Context.User.Id).AsTask();

        protected Task<User?> FindUserAsync(ulong id)
            => _context.Users.FindAsync(id).AsTask();

        protected Task<EventType?> FindEventType(string name)
            => _context.EventTypes.Where(et => et.Name == name)
                .FirstOrDefaultAsync();
        protected List<Event> FindEventsByDate(DateOnly date)
            => [.. _context.Events
                    .Include(e => e.EventType)
                    .Where(e => e.StartDt.Date == date
                        .ToDateTime(TimeOnly.MinValue).Date)];

        protected async Task<bool> EnsureExecutorHasAccessAsync(User? executor, Role targetRole)
        {
            if (executor is null || executor.Role <= targetRole)
            {
                await RespondAsync("you have no access.", ephemeral: true);
                return false;
            }
            return true;
        }

        protected async Task<bool> EnsureObjectExistsAsync(object? obj, string name)
        {
            if (obj is null)
            {
                await RespondAsync($"the {name} does not exist.", ephemeral: true);
                return false;
            }
            return true;
        }
    }
}