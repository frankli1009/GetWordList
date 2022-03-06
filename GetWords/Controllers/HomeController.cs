using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GetWords.Models;
using Microsoft.Extensions.Configuration;
using System.Web;
using Newtonsoft.Json;
using GetWords.Utilities;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;

namespace GetWords.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache _cache;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private string _getWordsAPIUrl = $"http://services.franklidev.com/api/gws";

        public HomeController(IConfiguration config, IHttpClientFactory clientFactory, ILogger<HomeController> logger, IMemoryCache cache)
        {
            _cache = cache;
            _config = config;
            _clientFactory = clientFactory;
            _logger = logger;
            _getWordsAPIUrl = _config.GetValue<string>("GetWordsAPIUrl", $"http://services.franklidev.com/api/gws");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(WordRequirement wr)
        {
            if (ModelState.IsValid)
            {
                Tuple<string, List<string>> result = await new GetWordsRequest(_clientFactory).OnGetWordsRequest(_getWordsAPIUrl, wr);
                ViewData["WordList"] = result.Item2 ?? new List<string>();
                ViewData["RequestError"] = result.Item1 ?? "OK";
                ViewData["CurPage"] = 1;
                string sk_wr = SessionKeys.SessionKey_Requirement(wr.Letters, wr.Length);
                ViewData["SessionKey_WR"] = sk_wr;
                if (result.Item2 != null)
                {
                    _cache.Set(sk_wr, result.Item2);
                }

                return View();
            }
            else return View();
        }

        public IActionResult WordsResult(int curPage, string sessionKey)
        {
            List<string> wordList = SessionKeys.GetSessionValue(_cache, sessionKey, new List<string>());
            ViewData["WordList"] = wordList;
            ViewData["CurPage"] = curPage;
            ViewData["SessionKey_WR"] = sessionKey;

            return PartialView();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
