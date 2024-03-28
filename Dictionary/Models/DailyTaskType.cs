using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
	public class DailyTaskType
	{
		public DailyTaskType()
		{
		}
        public int Id { get; set; }
        public int TypeId { get; set; }
        [Column(TypeName = "varchar(128)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }
    }
}

