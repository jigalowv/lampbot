using Microsoft.EntityFrameworkCore;

namespace lampbot.Entities
{
    [Index(nameof(Role))]
    public class User
    {
        public ulong Id { get; set; }
        public Role Role { get; set; } = Role.User;
    }
}