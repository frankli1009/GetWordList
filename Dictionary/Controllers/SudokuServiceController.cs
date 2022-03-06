using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Dictionary.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Controllers
{
    [ApiController]
    [Route("sudoku")]
    public class SudokuServiceController
    {
        private readonly ILogger<SudokuServiceController> _logger;
        private readonly WordDbContext _context;

        public SudokuServiceController(WordDbContext context, ILogger<SudokuServiceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("get/{id}")]
        public ActionResult GetSudoku(int id)
        {
            Sudoku s = _context.GetSudoku(id);
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpGet("random/{exceptId}")]
        public ActionResult RandomGetSudoku(int exceptId, string type)
        {
            string sql = $"spGetRandomSudoku {exceptId}, '{type}'";
            var s = _context.Sudokus.FromSqlRaw(sql) as IEnumerable<Sudoku>;
            if (s != null && s.Any())
            {
                return new OkObjectResult(s.First());
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpPost("add/{data}")]
        public async Task<ActionResult> PostSudoku(string data)
        {
            Sudoku sudoku = await _context.AddSudoku(data, _logger);
            if (sudoku != null)
            {
                return new OkObjectResult(sudoku);
            }
            else
            {
                return new BadRequestResult();
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult> PostSudoku(BatchWords datas)
        {
            int success = await _context.AddSudokuBatch(datas.Words, _logger);
            int count = datas.Words.Count();
            if (success > 0)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    if (success == count)
                    {
                        return new OkResult();
                    }
                    else
                    {
                        return new ConflictResult();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "failed to add/{0}", datas);
                    return new BadRequestResult();
                }
            }
            else
            {
                return new BadRequestResult();
            }
        }

        [HttpDelete("delete/{data}")]
        public async Task<IActionResult> DeleteSudoku(string data)
        {
            bool response = await _context.DeleteSudoku(data);
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
            return new OkObjectResult(new { name = "Sudoku", version = "1.0.0", author = "Frank Li" });
        }
    }
}
