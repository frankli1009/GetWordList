using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
	public class PageData<T>
	{
		public PageInfo Page { get; set; }
		public List<T> Data { get; set; }
	}
}

