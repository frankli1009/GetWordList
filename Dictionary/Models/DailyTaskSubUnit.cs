using System;
using System.Collections.Generic;
namespace Dictionary.Models
{
	public class DailyTaskSubUnit
	{
		public DailyTaskSubUnit()
		{
			DailyTaskSubExtraInfos = new List<DailyTaskSubExtraInfo>();
		}

        public DailyTaskSub DailyTaskSub { get; set; }
        public List<DailyTaskSubExtraInfo> DailyTaskSubExtraInfos { get; set; }
    }
}

