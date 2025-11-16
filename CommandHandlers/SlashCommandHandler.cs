using Discord;
using Discord.WebSocket;

namespace lampbot.CommandHandlers
{
    public class SlashCommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;

        public SlashCommandHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task InitAsync()
        {
            var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

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