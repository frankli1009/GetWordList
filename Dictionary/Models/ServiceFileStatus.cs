using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
	public class ServiceFileStatus
	{
		public ServiceFileStatus()
		{
		}
        public int Id { get; set; }
        public int StatusId { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string Name { get; set; }
    }
}

