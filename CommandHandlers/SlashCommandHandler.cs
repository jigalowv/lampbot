using Discord;
using Discord.WebSocket;
using lampbot.SlashCommands;
using Microsoft.Extensions.Configuration;

namespace lampbot.CommandHandlers
{
    public class SlashCommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _configuration;
        private readonly FunCommandList _funCommandList;
        private readonly UserCommandList _userCommandList;

        public SlashCommandHandler(
            DiscordSocketClient client,
            IConfiguration configuration,
            FunCommandList funCommandList,
            UserCommandList userCommandList)
        {
            _client = client;
            _configuration = configuration;
            _funCommandList = funCommandList;
            _userCommandList = userCommandList;
        }

        public async Task InitAsync()
        {
            var token = _configuration["DISCORD_TOKEN"];

            ArgumentException.ThrowIfNullOrEmpty(token);

            _client.SlashCommandExecuted += HandleSlashCommand;

            _client.Ready += async () =>
            {
                if (!ulong.TryParse(_configuration["GUILD_ID"], out ulong guildId))
                {
                    throw new ArgumentException("discord id is not ulong");
                }

                var guild = _client.GetGuild(guildId);

                ArgumentNullException.ThrowIfNull(guild);

                await guild.DeleteApplicationCommandsAsync();

                await _funCommandList.InitAsync(guild);
                await _userCommandList.InitAsync(guild);

                Console.WriteLine("bot is running");
            };

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        private async Task HandleSlashCommand(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "fun":
                    await _funCommandList.HandleSlashCommand(
                        command, command.Data.Options.First());
                    break;
                case "users":
                    await _userCommandList.HandleSlashCommand(
                        command, command.Data.Options.First());
                    break;
                default:
                    break;
            }
        }
    }
}