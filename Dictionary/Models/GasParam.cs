using System;
namespace Dictionary.Models
{
	public class GasParam : EnergyParam
    {
		public GasParam()
		{
		}

        public float Trans { get; set; }

        protected override string GetParams()
        {
            return $"{Quantity},{Price},{Trans}";
        }

        protected override string GetInfo()
        {
            return "Params: Quantity,Price,trans";
        }

        public override void CopyFrom(EnergyParam src)
        {
            base.CopyFrom(src);
            this.Trans = ((GasParam)src).Trans;
        }
    }
}

