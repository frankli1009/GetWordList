using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Dictionary.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public IActionResult GetWord(int id)
        {
            if (_context.Words.Any(w => w.Id == id))
            {
                Word w = _context.Words.Find(id);
                return new OkObjectResult(w);
            }

            return new NotFoundResult();
        }

        [HttpPost("add/{word}")]
        public async Task<ActionResult<OperationResult>> PostWord(string word)
        {
            Word w = new Word(word);
            try
            {
                _context.Words.Add(w);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetWord), new { id = w.Id }, w);
            }
            catch(Exception e)
            {
                List<Word> wds = _context.Words.Where(w => w.WordW.ToLower() == word.ToLower()).ToList();
                if(wds.Any())
                {
                    _logger.LogError(e, "failed to add/{0}, word already in db with Id {1}", word, wds.First().Id);
                }
                else
                {
                    _logger.LogError(e, "failed to add/{0}", word);
                }
                return new ConflictObjectResult(new OperationResult { Conflicts = wds });
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<OperationResult>> PostWord(BatchWords words)
        {
            bool changed = false;
            OperationResult or = new OperationResult();
            foreach (var word in words.Words)
            {
                Word w = new Word(word);
                try
                {
                    _context.Words.Add(w);
                    changed = true;
                    or.Oks.Add(w);
                }
                catch(Exception e)
                {
                    List<Word> wds = _context.Words.Where(w => w.WordW.ToLower() == word.ToLower()).ToList();
                    if (wds.Any())
                    {
                        _logger.LogError(e, "failed to add/{0}, word already in db with Id {1}", word, wds.First().Id);
                    }
                    else
                    {
                        _logger.LogError(e, "failed to add/{0}", word);
                    }
                    or.Conflicts.AddRange(wds);
                }
            }
            if(changed)
            {
                await _context.SaveChangesAsync();
            }
            return new ConflictObjectResult(or);
        }

        [HttpDelete("delete/{word}")]
        public async Task<ActionResult<OperationResult>> DeleteWord(string word)
        {
            if (_context.Words.Any(w => w.WordW.ToLower() == word.ToLower()))
            {
                List<Word> wds = _context.Words.Where(w => w.WordW.ToLower() == word.ToLower()).ToList();
                _context.Words.RemoveRange(wds);
                await _context.SaveChangesAsync();
                return new ConflictObjectResult(new OperationResult { Oks = wds });
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
