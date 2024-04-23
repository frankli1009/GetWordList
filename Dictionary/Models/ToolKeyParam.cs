using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictionary.Models
{
    public class ToolKeyParam
    {
		public ToolKeyParam()
		{
		}
        public int Id { get; set; }
        [Column(TypeName = "varchar(128)")]
        public string Category { get; set; }
        [Column(TypeName = "varchar(128)")]
        public string Key { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Parameters { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }
    }
}

