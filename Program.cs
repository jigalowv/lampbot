using Discord;
using Discord.WebSocket;
using lampbot.CommandHandlers;
using lampbot.Data;
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
    .AddSingleton<ICommandHandler, SlashCommandHandler>()
    .BuildServiceProvider();

await services.GetRequiredService<ICommandHandler>().InitAsync();

await Task.Delay(-1);