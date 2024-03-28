using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
	public class DailyTaskResponse
	{
		public DailyTaskResponse()
		{
			Errors = new List<string>();
        }

		public DailyTaskSubmit DailyTaskSubmit { get; set; }
		public List<string> Errors { get; set; }
	}
}

