using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace lampbot.SlashCommands
{
    public class FunCommandList : ISlashCommandList
    {
        public async Task InitAsync(SocketGuild guild)
        {
            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder()
                .WithName("fun")
                .WithDescription("example commands")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("echo")
                    .WithDescription("repeats what you type.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("value")
                        .WithDescription("value to repeat")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(true)
                    )
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("ping")
                    .WithDescription("sends 'pong'.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
                .Build()
            );
        }

        public async Task HandleSlashCommand(SocketSlashCommand command, SocketSlashCommandDataOption sub)
        {
            switch (sub.Name)
            {
                case "echo":
                    var lvl2 = sub.Options.First();
                    await EchoAsync(command, lvl2);
                    break;
                case "ping":
                    await PingAsync(command);
                    break;
                default:
                    break;
            }
        }

        private static async Task EchoAsync(SocketSlashCommand command, SocketSlashCommandDataOption lvl2)
        {
            await command.RespondAsync((string)lvl2.Value);
        }

        private static async Task PingAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("pong!");
        }
    }
}