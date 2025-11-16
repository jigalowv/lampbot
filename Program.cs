using Discord.WebSocket;
using lampbot.CommandHandlers;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddSingleton(new DiscordSocketConfig()
    {
        GatewayIntents = Discord.GatewayIntents.All
    })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton<ICommandHandler, SlashCommandHandler>()
    .BuildServiceProvider();

await services.GetRequiredService<ICommandHandler>().InitAsync();

await Task.Delay(-1);