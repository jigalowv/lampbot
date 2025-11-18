namespace lampbot.Entities
{
    public class User
    {
        public ulong Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.User;
    }
}