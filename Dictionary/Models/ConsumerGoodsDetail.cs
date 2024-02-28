using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictionary.Models
{
    public class ConsumerGoodsDetail
    {
		public ConsumerGoodsDetail()
		{
		}

        public int Id { get; set; }
        public float Quantity { get; set; }
        public float? Price { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }
        public DateTime Time { get; set; }

        public int ConsumerGoodsId { get; set; }
        public virtual ConsumerGoods ConsumerGoods { get; set; }
    }
}

