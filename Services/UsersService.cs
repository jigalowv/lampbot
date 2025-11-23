using lampbot.Data;
using lampbot.Entities;
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
            ulong userId,
            bool saveChanges = true)
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return GetUsersMessage(Constants.NotFound, executorId);
            
            var user = await _context.Users.FindAsync(userId);

            if (user is null)
                return GetUsersMessage(Constants.NotFound, userId);
            
            if (executor.Role <= user.Role)
                return GetUsersMessage(Constants.NoAccess, userId);

            _context.Users.Remove(user);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }

            return GetUsersMessage(Constants.Remove, userId);
        }

        public async Task<string> AddAsync(
            ulong executorId, 
            User user,
            bool saveChanges = true)
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return GetUsersMessage(Constants.NotFound, executorId);
            
            var dbUser = await _context.Users.FindAsync(user.Id);

            if (dbUser is null)
                return GetUsersMessage(Constants.AlreadyExists, user.Id);

            if (executor.Role <= user.Role)
                return GetUsersMessage(Constants.NoAccess, user.Id);

            await _context.Users.AddAsync(user);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }

            return GetUsersMessage(Constants.NoAccess, user.Id);
        }

        public async Task<string> ShowAsync(ulong userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user is null)
                return GetUsersMessage(Constants.NotFound, userId);
            
            return 
            $"""
                <@{user.Id}>
                - role: {user.Role}
                - id: {user.Id}
            """;
        }

        public async Task<string> SetRole(
            ulong executorId, 
            ulong userId, 
            Role newRole)
        {
            var executor = await _context.Users.FindAsync(executorId);
            
            if (executor is null)
                return GetUsersMessage(Constants.NotFound, executorId);
            
            var user = await _context.Users.FindAsync(userId);

            if (user is null)
                return GetUsersMessage(Constants.NotFound, userId);

            if (executor.Role <= user.Role ||
                executor.Role <= newRole)
                return GetUsersMessage(Constants.NoAccess, user.Id);

            user.Role = newRole;
            await _context.SaveChangesAsync();
            return GetUsersMessage(Constants.Update, user.Id);
        }

        private string GetUsersMessage(string field, object arg)
        {
            return string.Format(
                format: GetMessageSection("Users")[field]!, 
                arg0: arg);
        }
    }
}