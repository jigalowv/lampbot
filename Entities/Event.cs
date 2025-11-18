using System.ComponentModel.DataAnnotations.Schema;

namespace lampbot.Entities
{
    public class Event
    {
        public int Id { get; set; }
        [ForeignKey(nameof(EventType))]
        public int? EventTypeId { get; set; }
        public DateTime? StartDt { get; set; }
        public DateTime? EndDt { get; set; }

        public EventType? EventType { get; set; }
    }
}