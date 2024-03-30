using System;
using Dictionary.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dictionary.Utilities
{
	public class DailyTaskScheduleDetailComparer : IEqualityComparer<DailyTaskScheduleDetail>
    {
		public DailyTaskScheduleDetailComparer()
		{
		}

        public bool Equals([AllowNull] DailyTaskScheduleDetail x, [AllowNull] DailyTaskScheduleDetail y)
        {
            if (x.DailyTaskScheduleId == y.DailyTaskScheduleId && x.DailyTaskSubId == y.DailyTaskSubId) return true;
            else return false;
        }

        public int GetHashCode([DisallowNull] DailyTaskScheduleDetail obj)
        {
            return 0;
        }
    }
}

