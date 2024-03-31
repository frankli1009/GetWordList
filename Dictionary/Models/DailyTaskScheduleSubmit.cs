using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
	public class DailyTaskScheduleSubmit
	{
		public DailyTaskScheduleSubmit()
		{
			DailyTaskScheduleUnits = new List<DailyTaskScheduleUnit>();
		}

		public List<DailyTaskScheduleUnit> DailyTaskScheduleUnits { get; set; }
		public int TaskId { get; set; }
	}
}

