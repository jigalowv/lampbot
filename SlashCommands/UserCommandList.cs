using Discord;
using Discord.WebSocket;
using lampbot.Data;
using lampbot.Entities;

namespace lampbot.SlashCommands
{
    public class UserCommandList : ISlashCommandList
    {
        private readonly DataContext _context;

        public UserCommandList(DataContext context)
        {
            _context = context;
        }

        public async Task InitAsync(SocketGuild guild)
        {
            await guild.CreateApplicationCommandAsync(
                new SlashCommandBuilder()
                    .WithName("users")
                    .WithDescription("basic users operations.")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("add")
                        .WithDescription("add a user to the database.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("user")
                            .WithDescription("user to add.")
                            .WithType(ApplicationCommandOptionType.User)
                            .WithRequired(true))
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("role")
                            .WithDescription("user table role.")
                            .WithType(ApplicationCommandOptionType.Integer)
                            .WithRequired(true)
                            .AddChoice("куратор", (long)Role.Curator)
                            .AddChoice("ведущий", (long)Role.Lead)
                            .AddChoice("участник", (long)Role.User)
                        )
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("show")
                        .WithDescription("show a database user.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("user")
                            .WithDescription("user to show.")
                            .WithType(ApplicationCommandOptionType.User)))
                .Build());
        }

        public async Task HandleSlashCommand(SocketSlashCommand command, SocketSlashCommandDataOption sub)
        {
            switch (sub.Name)
            {
                case "add":
                    await AddAsync(
                        command: command, 
                        userToAdd: (SocketUser)sub.Options.First().Value, 
                        role: (Role)(long)sub.Options.ElementAt(1).Value);
                    break;
                case "show":
                    await ShowAsync(
                        command: command,
                        userToShow: (SocketUser?)sub.Options.FirstOrDefault()?.Value);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// adds a user to the data context
        /// </summary>
        /// <param name="command">executed command</param>
        /// <param name="userToAdd">user to add</param>
        /// <param name="role">user role</param>
        /// <returns></returns>
        private async Task AddAsync(SocketSlashCommand command, SocketUser userToAdd, Role role)
        {
            var executor = await _context.Users.FindAsync(command.User.Id);

            if (executor is null || executor.Role <= role)
            {
                await command.RespondAsync("you have no access.", ephemeral: true);
                return;
            }

            if (await _context.Users.FindAsync(userToAdd.Id) is not null)
            {
                await command.RespondAsync("user already exists.", ephemeral: true);
                return;
            }

            await _context.AddAsync(new User()
            {
               Id = userToAdd.Id,
               Name = userToAdd.Username,
               Role = role 
            });

            await _context.SaveChangesAsync();
            await command.RespondAsync("user added.", ephemeral: true);
        }

        private async Task ShowAsync(SocketSlashCommand command, SocketUser? userToShow)
        {
            var id = userToShow is null ? command.User.Id : userToShow.Id;

            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                await command.RespondAsync("user does not exist.", ephemeral: true);
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle(user.Name)
                .WithFooter(user.Id.ToString())
                .AddField("role", user.Role)
                .Build();
            
            await command.RespondAsync(ephemeral: true, embed: embed);
        }
    }
}