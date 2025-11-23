using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lampbot.Entities
{
    [Index(nameof(UserId), nameof(EventId))]
    [Index(nameof(EventId))]
    public class Participation
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }
        
        [ForeignKey(nameof(User))]
        public ulong UserId { get; set; }
        public bool IsActive { get; set; }

        public Event? Event { get; set; }
        public User? User { get; set; }
    }
}