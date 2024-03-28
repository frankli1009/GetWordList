using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
	public class DailyTaskSub
	{
		public DailyTaskSub()
		{
		}
        public int Id { get; set; }
        public int WorkLoad { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }

        public int DailyTaskId { get; set; }
        public virtual DailyTask DailyTask { get; set; }
    }
}

