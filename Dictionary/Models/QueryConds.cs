﻿using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
	public class QueryConds
	{
		public QueryConds()
		{
			_QueryParams = new List<string>();
		}

		private List<string> _QueryParams;

        public int QueryType { get; set; }
		public List<string> QueryParams
		{
			get => _QueryParams;
			set
			{
				_QueryParams.Clear();
				_QueryParams.AddRange(value);
			}
		}
		public int QueryPage { get; set; }
	}

	public enum QueryType
	{
		ByDate = 1,
		ById = 2,
		ByName = 3,
		ByCombination = 4
	}

	public enum QueryDateType
	{
		ByStartDate = 1,
		ByEndDate = 2,
		ByBothStartAndEndDate = 3,
        ByCreateTime = 4,
        ByDoneTime = 5
    }
}

