using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictionary.Models
{
    public class ConsumerGoods
    {
		public ConsumerGoods()
		{
		}

        public int Id { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }
    }
}

