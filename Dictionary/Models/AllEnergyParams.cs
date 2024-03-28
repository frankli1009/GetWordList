using System;
namespace Dictionary.Models
{
	public class AllEnergyParams
	{
		public AllEnergyParams()
		{
		}

		public EnergyParam ElectricParameter { get; set; }
		public GasParam GasParameter { get; set; }
	}
}

