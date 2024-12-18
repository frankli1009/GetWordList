using System;
namespace Dictionary.Models
{
	public class PageInfo
	{
		public PageInfo()
		{
			FirstPage = false;
			PrevPage = false;
			NextPage = false;
			LastPage = false;
			Prev2Page = 0;
			Prev1Page = 0;
			Next1Page = 0;
			Next2Page = 0;
		}

		public int CurrentPage { get; set; }
		public int CountPerPage { get; set; }
		public int TotalPageCount { get; set; }
		public int TotalCount { get; set; }
		public bool FirstPage { get; set; }
		public bool PrevPage { get; set; }
		public bool NextPage { get; set; }
		public bool LastPage { get; set; }
		public int Prev2Page { get; set; }
		public int Prev1Page { get; set; }
		public int Next1Page { get; set; }
		public int Next2Page { get; set; }
	}

}

