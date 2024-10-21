using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
	public class DailyTaskSubmit
	{
		public DailyTaskSubmit()
		{
			DailyTaskSubUnits = new List<DailyTaskSubUnit>();
		}

		public DailyTask DailyTask { get; set; }
		public List<DailyTaskSubUnit> DailyTaskSubUnits { get; set; }
	}
}

