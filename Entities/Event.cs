using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lampbot.Entities
{
    [Index(nameof(StartDate))]
    public class Event
    {
        public int Id { get; set; }
        [ForeignKey(nameof(EventType))]
        public int EventTypeId { get; set; }
        public DateOnly StartDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public DateTime? EndDt { get; set; }
        public EventType? EventType { get; set; }
    }
}