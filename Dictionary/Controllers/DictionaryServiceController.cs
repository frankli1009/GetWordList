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
            Word w = _context.GetWord(id);
            if (w == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(w);
            }
        }

        [HttpPost("add/{word}")]
        public async Task<ActionResult<OperationResult<Word>>> PostWord(string word)
        {
            OperationResult<Word> or = await _context.AddWord(word, _logger);
            if (or.Conflicts.Any())
            {
                return new ConflictObjectResult(or.Conflicts[0]);
            }
            else if (or.Oks.Any())
            {
                return new OkObjectResult(or.Oks[0]);
            }
            else
            {
                return new BadRequestResult();
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<OperationResult<Word>>> PostWord(BatchWords words)
        {
            OperationResult<Word> or = await _context.AddWordBatch(words, _logger);
            if (or.Conflicts.Any())
            {
                return new ConflictObjectResult(or);
            }
            else if (or.NotFounds.Any())
            {
                return new BadRequestObjectResult(or);
            }
            else if (or.Oks.Any())
            {
                return new OkObjectResult(or);
            }
            else
            {
                return new BadRequestObjectResult(or);
            }
        }

        [HttpDelete("delete/{word}")]
        public async Task<IActionResult> DeleteWord(string word)
        {
            bool response = await _context.DeleteWord(word);
            if (response)
            {
                return new OkResult();
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("info")]
        public IActionResult Info()
        {
            return new OkObjectResult(new { name = "Dictionary", version = "2.0.0", author = "Frank Li" });
        }
    }
}
