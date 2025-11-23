using Discord.Interactions;
using Discord.WebSocket;
using lampbot.Entities;
using lampbot.Services;

namespace lampbot.SlashCommands
{
    [Group("users", "basic users' operations.")]
    public sealed class UsersModule : ModuleBase
    {
        private readonly UsersService _usersService;

        public UsersModule(UsersService usersService)
        {
            _usersService = usersService;
        }

        [SlashCommand("add", "adds a user to the database.")]
        public async Task AddAsync(
            [Summary("user", "user to add.")] SocketUser user,
            [Summary("role", "the user's database role.")] Role role = Role.User)
        {
            var userObj = new User()
            {
                Id = user.Id,
                Role = role
            };

            var response = await _usersService.AddAsync(Context.User.Id, userObj);

            await RespondAsync(response, ephemeral: true);
        }

        [SlashCommand("remove", "remove the user from the database.")]
        public async Task RemoveAsync([Summary("user", "user to remove.")] SocketUser user)
        {
            var response = await _usersService.RemoveAsync(Context.User.Id, user.Id);

            await RespondAsync(response, ephemeral: true);
        }

        [SlashCommand("show", "show the user's properties.")]
        public async Task ShowAsync([Summary("user", "user to show.")] SocketUser? user = null)
        {
            var userToShow = user ?? Context.User;

            var response = await _usersService.ShowAsync(userToShow.Id);

            await RespondAsync(response, ephemeral: true);
        }

        [SlashCommand("set-role", "update the user's role.")]
        public async Task UpdateRoleAsync(
            [Summary("user", "user to update.")] SocketUser user,
            [Summary("role", "user's new role.")] Role role)
        {
            var response = await _usersService.SetRole(Context.User.Id, user.Id, role);

            await RespondAsync(response, ephemeral: true);
        }
    }
}
