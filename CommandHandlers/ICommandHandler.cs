using Discord.WebSocket;

namespace lampbot.CommandHandlers
{
    public interface ICommandHandler
    {
        public Task InitAsync();
    }
}