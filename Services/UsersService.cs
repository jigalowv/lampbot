using Discord;
using lampbot.Data;
using lampbot.Entities;
using lampbot.Extensions;
using Microsoft.Extensions.Configuration;

namespace lampbot.Services
{
    public sealed class UsersService : ServiceBase
    {
        public UsersService(
            DataContext context,
            IConfiguration config)
            : base(context, config)
        { }

        public async Task<string> RemoveAsync(
            ulong executorId, 
            ulong userId)
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), $"<@{executorId}>");
            
            var user = await _context.Users.FindAsync(userId);

            if (user is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), $"<@{userId}>");
            
            if (executor.Role <= user.Role)
                return _config.GetResponse(Constants.NoAccess, nameof(User), $"<@{userId}>");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return _config.GetResponse(Constants.Remove, nameof(User), $"<@{userId}>");
        }

        public async Task<string> AddAsync(
            ulong executorId, 
            User user)
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), $"<@{executorId}>");
            
            var dbUser = await _context.Users.FindAsync(user.Id);

            if (dbUser is null)
                return _config.GetResponse(Constants.AlreadyExists, nameof(User), $"<@{user.Id}>");

            if (executor.Role <= user.Role)
                return _config.GetResponse(Constants.NoAccess, nameof(User), $"<@{user.Id}>");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            return _config.GetResponse(Constants.NoAccess, nameof(User), $"<@{user.Id}>");
        }

        public async Task<string> ShowAsync(ulong userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), $"<@{userId}>");
            
            return $"<@{user.Id}> ({user.Role})";
        }

        public async Task<string> SetRole(
            ulong executorId, 
            ulong userId, 
            Role newRole)
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), $"<@{executorId}>");
            var user = await _context.Users.FindAsync(userId);

            if (user is null)
                return _config.GetResponse(Constants.NotFound, nameof(User), $"<@{userId}>");

            if (executor.Role <= user.Role ||
                executor.Role <= newRole)
                return _config.GetResponse(Constants.NoAccess, nameof(User), $"<@{user.Id}>");

            user.Role = newRole;
            await _context.SaveChangesAsync();

            return _config.GetResponse(Constants.Update, nameof(User), $"<@{user.Id}>");
        }
    }
}