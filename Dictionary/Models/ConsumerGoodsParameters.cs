using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictionary.Models
{
	public class ConsumerGoodsParameters
	{
		public ConsumerGoodsParameters()
		{
		}

        public int Id { get; set; }
        public int StartDay { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Params { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Info { get; set; }
        public DateTime Time { get; set; }
        public bool Valid { get; set; }

        public int ConsumerGoodsId { get; set; }
        public virtual ConsumerGoods ConsumerGoods { get; set; }
    }
}

