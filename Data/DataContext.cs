using lampbot.Entities;
using Microsoft.EntityFrameworkCore;

namespace lampbot.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}