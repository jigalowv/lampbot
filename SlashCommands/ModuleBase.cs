using Discord.Interactions;
using lampbot.Data;
using lampbot.Entities;

namespace lampbot.SlashCommands
{
    public class ModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        protected readonly DataContext _context;

        public ModuleBase(DataContext context)
        {
            _context = context;
        }

        protected Task<User?> GetExecutorAsync()
            => _context.Users.FindAsync(Context.User.Id).AsTask();

        protected Task<User?> FindUserAsync(ulong id)
            => _context.Users.FindAsync(id).AsTask();

        protected async Task<bool> EnsureUserExistsAsync(User? user)
        {
            if (user is null)
            {
                await RespondAsync("the user does not exist.", ephemeral: true);
                return false;
            }
            return true;
        }

        protected async Task<bool> EnsureExecutorHasAccessAsync(User? executor, Role targetRole)
        {
            if (executor is null || executor.Role <= targetRole)
            {
                await RespondAsync("you have no access.", ephemeral: true);
                return false;
            }
            return true;
        }
    }
}