using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GetWordsAPI.Controllers
{
    [ApiController]
    [Route("gws")]
    public class GetWordsServiceController : ControllerBase
    {
        private readonly ILogger<GetWordsServiceController> _logger;

        public GetWordsServiceController(ILogger<GetWordsServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{src}/{len:int}/{extra?}")]
        public Object Get(string src, int len, string extra, string rejected)
        {
            var words = new GetWords(src.ToLower(), len, extra, rejected).Words;
            return words;
        }
    }
}
