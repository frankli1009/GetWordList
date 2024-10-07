using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
    public class DailyTaskSuspended
    {
        public DailyTaskSuspended()
        {
        }
        public int Id { get; set; }
        public int SuspendedId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }

        public int DailyTaskId { get; set; }
        public virtual DailyTask DailyTask { get; set; }
    }
}

