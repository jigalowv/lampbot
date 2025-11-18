using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using lampbot.Data;
using lampbot.Entities;

namespace lampbot.SlashCommands
{
    [Group("users", "basic users' operations.")]
    public class UsersModule : ModuleBase
    {
        public UsersModule(DataContext context)
            : base(context)
        { }

        private async Task<bool> EnsureUserExistsAsync(User? user)
        {
            if (user is null)
            {
                await RespondAsync("the user does not exist.", ephemeral: true);
                return false;
            }
            return true;
        }

        private async Task<bool> EnsureExecutorHasAccessAsync(User? executor, Role targetRole)
        {
            if (executor is null || executor.Role <= targetRole)
            {
                await RespondAsync("you have no access.", ephemeral: true);
                return false;
            }
            return true;
        }

        [SlashCommand("add", "adds a user to the database.")]
        public async Task AddAsync(
            [Summary("user", "user to add.")] SocketUser user,
            [Summary("role", "the user's database role.")] Role role = Role.User)
        {
            var executor = await GetExecutorAsync();

            if (!await EnsureExecutorHasAccessAsync(executor, role))
                return;

            if (await FindUserAsync(user.Id) is not null)
            {
                await RespondAsync("the user already exists.", ephemeral: true);
                return;
            }

            await _context.AddAsync(new User
            {
                Id = user.Id,
                Name = user.Username,
                Role = role
            });

            await _context.SaveChangesAsync();
            await RespondAsync("the user has been added.", ephemeral: true);
        }

        [SlashCommand("remove", "remove the user from the database.")]
        public async Task RemoveAsync([Summary("user", "user to remove.")] SocketUser user)
        {
            var executor = await GetExecutorAsync();
            var dbUser = await FindUserAsync(user.Id);

            if (!await EnsureUserExistsAsync(dbUser))
                return;

            if (!await EnsureExecutorHasAccessAsync(executor, dbUser!.Role))
                return;

            _context.Users.Remove(dbUser);
            await _context.SaveChangesAsync();
            await RespondAsync("the user has been successfully removed.", ephemeral: true);
        }

        [SlashCommand("show", "show the user's properties.")]
        public async Task ShowAsync([Summary("user", "user to show.")] SocketUser? user = null)
        {
            var id = user?.Id ?? Context.User.Id;
            var dbUser = await FindUserAsync(id);

            if (!await EnsureUserExistsAsync(dbUser))
                return;

            var embed = new EmbedBuilder()
                .WithTitle(dbUser!.Name)
                .WithFooter("id: " + dbUser.Id)
                .AddField("role", dbUser.Role, true)
                .Build();

            await RespondAsync(ephemeral: true, embed: embed);
        }

        [SlashCommand("update-role", "update the user's role.")]
        public async Task UpdateRoleAsync(
            [Summary("user", "user to update.")] SocketUser user,
            [Summary("role", "user's new role.")] Role role)
        {
            var executor = await GetExecutorAsync();
            var dbUser = await FindUserAsync(user.Id);

            if (!await EnsureUserExistsAsync(dbUser))
                return;

            if (executor is null ||
                executor.Role <= dbUser!.Role ||
                executor.Role <= role)
            {
                await RespondAsync("you have no access.", ephemeral: true);
                return;
            }

            dbUser.Role = role;
            await _context.SaveChangesAsync();
            await RespondAsync("the user's role has been updated.", ephemeral: true);
        }
    }
}
