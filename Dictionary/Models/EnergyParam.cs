using System;
namespace Dictionary.Models
{
	public class EnergyParam
	{
		public EnergyParam()
		{
		}

        public const int ElectricId = 1;
        public const int GasId = 2;
        public int ConsumerGoodsId { get; set; }
        public int StartDay { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string Info => GetInfo();

        public string Params => GetParams();

        protected virtual string GetParams()
        {
            return $"{Quantity},{Price}";
        }

        protected virtual string GetInfo()
        {
            return "Params: Quantity,Price";
        }

        public virtual void CopyFrom(EnergyParam src)
        {
            this.ConsumerGoodsId = src.ConsumerGoodsId;
            this.StartDay = src.StartDay;
            this.Quantity = src.Quantity;
            this.Price = src.Price;
        }
    }
}

