using Discord.Interactions;

namespace lampbot.SlashCommands
{
    [Group("fun", "example commands")]
    public class FunModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand(name: "echo", description: "repeats what you type")]
        public async Task EchoAsync(
            [Summary(name: "value", description: "value to repeat.")] string value)
        {
            await RespondAsync(value);
        }

        [SlashCommand(name: "ping", description: "sends 'pong'")]
        public async Task PingAsync()
        {
            await RespondAsync("pong!");
        }
    }
}