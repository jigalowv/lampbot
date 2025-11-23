using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using lampbot;
using lampbot.Data;
using lampbot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Build();

var services = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddDbContext<DataContext>(options =>
    {
        options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
    })
    .AddSingleton(new DiscordSocketConfig()
    {
        GatewayIntents = GatewayIntents.Guilds |
                         GatewayIntents.GuildMessages |
                         GatewayIntents.DirectMessages
    })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton(sp =>
    {
        var client = sp.GetRequiredService<DiscordSocketClient>();
        return new InteractionService(client.Rest);
    })
    .AddSingleton<UsersService>()
    .AddSingleton<EventTypeService>()
    .AddSingleton<EventService>()
    .AddSingleton<InteractionHandler>()
    .BuildServiceProvider();

await services.GetRequiredService<InteractionHandler>().InitAsync();

await Task.Delay(-1);