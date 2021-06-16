using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GetWords.Utilities
{
    public class Pagination
    {
        public readonly int PageNoMaxVisibleCount = 7;
        public readonly int WordCountPerRow = 4;
        public readonly int RowCountPerPage = 8;
        public int WordCountPerPage => WordCountPerRow * RowCountPerPage;

        private readonly List<string> _words;
        private readonly int _curPage;
        private readonly int _pageCount;
        private readonly int _pageNoVisibleCount;
        private readonly int _pageNoBegins;
        private readonly IUrlHelper _url;
        private readonly string _sessionKey;

        public Pagination(List<string> words, int curPage, IUrlHelper url, string sessionKey)
        {
            _sessionKey = sessionKey;
            _url = url;
            _words = words;
            _curPage = curPage;
            _pageCount = _words.Count / WordCountPerPage;
            if (_pageCount * WordCountPerPage < _words.Count) _pageCount = _pageCount + 1;
            _pageNoVisibleCount = _pageCount >= PageNoMaxVisibleCount ? PageNoMaxVisibleCount : _pageCount;
            _pageNoBegins = _curPage - PageNoMaxVisibleCount / 2;
            if (_pageNoBegins < 1)
            {
                _pageNoBegins = 1;
            }
            else if (_pageNoBegins > _pageCount - _pageNoVisibleCount + 1)
            {
                _pageNoBegins = _pageCount - _pageNoVisibleCount + 1;
            }
        }

        public List<string> Words
        {
            get
            {
                return _words;
            }
        }

        public int CurPage
        {
            get
            {
                return _curPage;
            }
        }

        public int PageCount
        {
            get
            {
                return _pageCount;
            }
        }

        public int PageNoVisibleCount
        {
            get
            {
                return _pageNoVisibleCount;
            }
        }

        public bool PrevPageDisabled
        {
            get
            {
                return _curPage == 1;
            }
        }

        public bool NextPageDisabled
        {
            get
            {
                return _curPage == _pageCount;
            }
        }

        public string PrevPageDisabledClass
        {
            get
            {
                return PrevPageDisabled ? "disabled" : "";
            }
        }

        public string NextPageDisabledClass
        {
            get
            {
                return NextPageDisabled ? "disabled" : "";
            }
        }

        public string PageNoDisabledClass(int pageNo)
        {
            if (pageNo == _curPage) return "disabled";
            else return "";
        }

        public string PageNoActiveClass(int pageNo)
        {
            if (pageNo == _curPage) return "active";
            else return "";
        }

        public int PageNoBegins
        {
            get
            {
                return _pageNoBegins;
            }
        }

        public int PageNoEnds
        {
            get
            {
                return _pageNoBegins + _pageNoVisibleCount - 1;
            }
        }

        public List<string> WordsOnCurPage
        {
            get
            {
                return _words.GetRange((_curPage - 1) * WordCountPerPage,
                    _curPage * WordCountPerPage >= _words.Count ?
                    _words.Count - (_curPage - 1) * WordCountPerPage :
                    WordCountPerPage);
            }
        }

        public int RowCount
        {
            get
            {
                return WordsOnCurPage.Count / WordCountPerRow * WordCountPerRow < WordsOnCurPage.Count ?
                    WordsOnCurPage.Count / WordCountPerRow + 1 : WordsOnCurPage.Count / WordCountPerRow;
            }
        }

        public int WordNoBegins
        {
            get
            {
                return (_curPage - 1) * WordCountPerPage + 1;
            }
        }
         
        public HtmlString RenderPaination()
        {
            string words = JsonConvert.SerializeObject(_words);

            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class=\"pagination justify-content-end padding-right-20 text-nowrap\">");

            if (PrevPageDisabled)
            {
                sb.Append($"<li class=\"page-item {PrevPageDisabledClass}\"><a class=\"page-link\" href=\"#\"><b>|&lt;&lt;</b></a></li>");
                sb.Append($"<li class=\"page-item {PrevPageDisabledClass}\"><a class=\"page-link\" href=\"#\"><b>&lt;&lt;</b></a></li>");
            }
            else
            {
                string linkUrlFirstPage = _url.Action("WordsResult", "Home", new { curpage = 1, sessionkey = _sessionKey });
                sb.Append($"<li class=\"page-item\"><a class=\"page-link page-link-active\" href=\"#\" data-url=\"{linkUrlFirstPage}\"><b>|&lt;&lt;</b></a></li>");
                string linkUrlPrevPage = _url.Action("WordsResult", "Home", new { curpage = _curPage - 1, sessionkey = _sessionKey });
                sb.Append($"<li class=\"page-item\"><a class=\"page-link page-link-active\" href=\"#\" data-url=\"{linkUrlPrevPage}\"><b>&lt;&lt;</b></a></li>");
            }

            for (int i = PageNoBegins; i <= PageNoEnds; i++)
            {
                string pageNoActiveClass = PageNoActiveClass(i);
                if (i == _curPage)
                {
                    sb.Append($"<li class=\"page-item {pageNoActiveClass}\"><a class=\"page-link\" href=\"#\">{i}</a></li>");
                }
                else
                {
                    string linkUrl = _url.Action("WordsResult", "Home", new { curpage = i, sessionkey = _sessionKey });
                    sb.Append($"<li class=\"page-item {pageNoActiveClass}\"><a class=\"page-link page-link-active\" href=\"#\" data-url=\"{linkUrl}\">{i}</a></li>");
                }
            }

            if (NextPageDisabled)
            {
                sb.Append($"<li class=\"page-item {NextPageDisabledClass}\"><a class=\"page-link\" href=\"#\"><b>&gt;&gt;</b></a></li>");
                sb.Append($"<li class=\"page-item {NextPageDisabledClass}\"><a class=\"page-link\" href=\"#\"><b>&gt;&gt;|</b></a></li>");
            }
            else
            {
                string linkUrlNextPage = _url.Action("WordsResult", "Home", new { curpage = _curPage+1, sessionkey = _sessionKey });
                sb.Append($"<li class=\"page-item\"><a class=\"page-link page-link-active\" href=\"#\" data-url=\"{linkUrlNextPage}\"><b>&gt;&gt;</b></a></li>");
                string linkUrlLastPage = _url.Action("WordsResult", "Home", new { curpage = _pageCount, sessionkey = _sessionKey });
                sb.Append($"<li class=\"page-item\"><a class=\"page-link page-link-active\" href=\"#\" data-url=\"{linkUrlLastPage}\"><b>&gt;&gt;|</b></a></li>");
            }

            sb.Append("</ul>");

            string result = sb.ToString();
            return new HtmlString(result);
        }
    }
}
