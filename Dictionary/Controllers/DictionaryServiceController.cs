using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Dictionary.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dictionary.Controllers
{
    [ApiController]
    [Route("dictionary")]
    public class DictionaryServiceController : ControllerBase
    {
        private readonly ILogger<DictionaryServiceController> _logger;
        private readonly WordDbContext _context;

        public DictionaryServiceController(WordDbContext context, ILogger<DictionaryServiceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("get/{src}/{len:int}/{extra?}")]
        public IActionResult GetWord(string src, int len, string extra)
        {
            var words = new GetWords(_context, src.ToLower(), len, extra).Words;
            if (words.Any())
            {
                return new OkObjectResult(words);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("get/{id:int}")]
        [ActionName("GetWord")]
        public ActionResult GetWord(int id)
        {
            return new ConflictObjectResult(_context.GetWord(id));
        }

        [HttpPost("add/{word}")]
        public async Task<ActionResult<OperationResult>> PostWord(string word)
        {
            return new ConflictObjectResult(await _context.AddWord(word, _logger));
        }

        [HttpPost("add")]
        public async Task<ActionResult<OperationResult>> PostWord(BatchWords words)
        {
            return new ConflictObjectResult(await _context.AddWordBatch(words, _logger));
        }

        [HttpDelete("delete/{word}")]
        public async Task<ActionResult<OperationResult>> DeleteWord(string word)
        {
            return new ConflictObjectResult(await _context.DeleteWord(word));
        }

        [HttpGet("info")]
        public IActionResult Info()
        {
            return new OkObjectResult(new { name = "Dictionary", version = "1.0.0", author = "Frank Li" });
        }
    }
}
