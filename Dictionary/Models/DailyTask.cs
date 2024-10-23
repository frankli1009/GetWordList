using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
	public class DailyTask
	{
		public DailyTask()
		{
		}
        public int Id { get; set; }
        [Column(TypeName = "varchar(128)")]
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }
        public DateTime Time { get; set; }
        public int DailyTaskStatusId { get; set; }
        public int DoneLeastWorkload { get; set; }
        public int Suspended { get; set; }

        public int DailyTaskTypeId { get; set; }
        public virtual DailyTaskType DailyTaskType { get; set; }
        public int DailyTaskThemeId { get; set; }
        public virtual DailyTaskTheme DailyTaskTheme { get; set; }
    }
}

