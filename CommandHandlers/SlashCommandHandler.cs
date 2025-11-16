using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace lampbot.CommandHandlers
{
    public class SlashCommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _configuration;

        public SlashCommandHandler(
            DiscordSocketClient client,
            IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task InitAsync()
        {
            var token = _configuration["DISCORD_TOKEN"];

            ArgumentException.ThrowIfNullOrEmpty(token);

            _client.Ready += () =>
            {
                Console.WriteLine("bot is running");
                return Task.CompletedTask;
            };

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        private async Task HandleSlashCommand(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                default:
                break;
            }
        }
    }
}