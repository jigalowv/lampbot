using Discord.WebSocket;
using lampbot.CommandHandlers;
using lampbot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddDbContext<DataContext>(options =>
    {
        options.UseSqlite("Data source=lamp.db");
    })
    .AddSingleton(new DiscordSocketConfig()
    {
        GatewayIntents = Discord.GatewayIntents.All
    })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton<ICommandHandler, SlashCommandHandler>()
    .BuildServiceProvider();

await services.GetRequiredService<ICommandHandler>().InitAsync();

await Task.Delay(-1);