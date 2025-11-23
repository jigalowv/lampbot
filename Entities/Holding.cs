using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lampbot.Entities
{
    [Index(nameof(LeadId), nameof(EventId))]
    [Index(nameof(EventId))]
    public class Holding
    {
        public int Id { get; set; }
        
        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }
        
        [ForeignKey(nameof(Lead))]
        public ulong LeadId { get; set; }
        public bool IsMain { get; set; }

        public Event? Event { get; set; }
        public User? Lead { get; set; }
    }
}