using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
    public class DailyTaskScheduleDetail
    {
        public DailyTaskScheduleDetail()
        {
        }
        public int Id { get; set; }
        public int DailyTaskSubId { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }
        public int OrderId { get; set; }

        public int DailyTaskScheduleId { get; set; }
        public virtual DailyTaskSchedule  DailyTaskSchedule { get; set; }
        public int DailyTaskStatusId { get; set; }
        public virtual DailyTaskStatus DailyTaskStatus { get; set; }
    }
}

