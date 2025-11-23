using Microsoft.EntityFrameworkCore;

namespace lampbot.Entities
{
    [Index(nameof(Name))]
    public class EventType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}