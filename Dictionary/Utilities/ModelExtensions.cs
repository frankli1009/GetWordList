using System;
using Dictionary.Models;

namespace Dictionary.Utilities
{
	public static class ModelExtensions
	{
		public static ConsumerGoodsDetail CopyFrom(this ConsumerGoodsDetail detail, ConsumerGoodsDetail src)
		{
			detail.ConsumerGoodsId = src.ConsumerGoodsId;
			detail.Price = src.Price;
			detail.Quantity = src.Quantity;
			detail.Time = src.Time;
            detail.Info = src.Info; 

            return detail;
		}
	}
}

