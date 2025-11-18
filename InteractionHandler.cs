using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace lampbot
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _configuration;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;

        public InteractionHandler(
            DiscordSocketClient client,
            InteractionService interactions,
            IServiceProvider services,
            IConfiguration configuration)
        {
            _client = client;
            _interactions = interactions;
            _configuration = configuration;
            _services = services;
        }

        public async Task InitAsync()
        {
            var token = _configuration["DISCORD_TOKEN"];

            ArgumentException.ThrowIfNullOrEmpty(token);

            _client.InteractionCreated += HandleInteractionAsync;

            _client.Ready += async () =>
            {
                if (!ulong.TryParse(_configuration["GUILD_ID"], out ulong guildId))
                {
                    throw new ArgumentException("discord id is not ulong");
                }

                var guild = _client.GetGuild(guildId);

                ArgumentNullException.ThrowIfNull(guild);

                await guild.DeleteApplicationCommandsAsync();

                await _interactions.AddModulesAsync(typeof(Program).Assembly, _services);

                await RegisterCommandsAsync(guildId);                 

                Console.WriteLine("bot is running");
            };

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        private async Task RegisterCommandsAsync(ulong guildId)
        {
            await _interactions.RegisterCommandsToGuildAsync(guildId);
        }
        
        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _services);
        }
    }
}