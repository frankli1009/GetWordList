using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dictionary.Models;

namespace Dictionary.Utilities
{
	public class DailyTaskSubComparer : IEqualityComparer<DailyTaskSub>
    {
		public DailyTaskSubComparer()
		{
		}

        public bool Equals([AllowNull] DailyTaskSub x, [AllowNull] DailyTaskSub y)
        {
            if (x.Id == y.Id && x.DailyTaskId == y.DailyTaskId) return true;
            else return false;
        }

        public int GetHashCode([DisallowNull] DailyTaskSub obj)
        {
            return 0;
        }
    }
}

