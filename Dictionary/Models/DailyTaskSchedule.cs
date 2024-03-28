using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
    public class DailyTaskSchedule
    {
        public DailyTaskSchedule()
        {
        }
        public int Id { get; set; }
        public DateTime ActDate { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }

        public int DailyTaskId { get; set; }
        public virtual DailyTask DailyTask { get; set; }
    }
}
