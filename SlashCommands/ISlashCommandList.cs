using Discord.WebSocket;

namespace lampbot.SlashCommands
{
    public interface ISlashCommandList
    {
        public Task InitAsync(SocketGuild guild);
        public Task HandleSlashCommand(SocketSlashCommand command, SocketSlashCommandDataOption sub);     
    }
}