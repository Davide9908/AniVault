
using System.ComponentModel.DataAnnotations.Schema;

namespace AniVault.Database
{
    [Table("scheduled_task")]
    public class ScheduledTask
    {
        public int ScheduledTaskId { get; set; }
        public string TaskName { get; set; }
        public DateTime? LastStart { get; set; }
        public DateTime? LastFinish { get; set; }
        public bool Enabled { get; set; }
        public int? IntervalSeconds { get; set; }

    }
}
