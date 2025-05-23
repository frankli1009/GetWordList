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

		public static Delivery CopyFrom(this Delivery delivery, Delivery src)
		{
			delivery.ProductName = src.ProductName;
			delivery.OrderTime = src.OrderTime;
			delivery.Info = src.Info;
			delivery.DeliveryNo = src.DeliveryNo;
			delivery.StatusId = src.StatusId;
			delivery.Time = src.Time;

			if (src.StatusId == 1)
			{
				delivery.ReceiveTime = null;
				delivery.CancelTime = null;

            }
			else if (src.StatusId == 4)
			{
				delivery.ReceiveTime = src.ReceiveTime;
				delivery.CancelTime = null;
			}
			else if (src.StatusId == 5)
			{
				delivery.ReceiveTime = null;
				delivery.CancelTime = src.CancelTime;
            }

			return delivery;
		}
	}
}

