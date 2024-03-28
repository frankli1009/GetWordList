using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
	public class DailyTaskSubmit
	{
		public DailyTaskSubmit()
		{
			DailyTaskSubs = new List<DailyTaskSub>();
		}

		public DailyTask DailyTask { get; set; }
		public List<DailyTaskSub> DailyTaskSubs { get; set; }
	}
}

