using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
	public class DailyTaskScheduleUnit
	{
		public DailyTaskScheduleUnit()
		{
			DailyTaskScheduleDetails = new List<DailyTaskScheduleDetail>();
        }

		public DailyTaskSchedule DailyTaskSchedule { get; set; }
		public List<DailyTaskScheduleDetail> DailyTaskScheduleDetails { get; set; }
	}
}

