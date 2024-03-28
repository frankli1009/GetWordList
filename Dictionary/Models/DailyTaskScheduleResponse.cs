using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
	public class DailyTaskScheduleResponse
	{
		public DailyTaskScheduleResponse()
		{
			Errors = new List<string>();
		}

		public DailyTaskScheduleSubmit DailyTaskScheduleSubmit { get; set; }
		public List<string> Errors { get; set; }
    }
}

