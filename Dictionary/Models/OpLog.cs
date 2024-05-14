using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictionary.Models
{
	public class OpLog
	{
		public OpLog()
		{
		}

        public int Id { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string Key { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string SubKey { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }
        public DateTime LogTime { get; set; }

        public int OpLogLevelId { get; set; }
        public virtual OpLogLevel OpLogLevel { get; set; }
    }
}

