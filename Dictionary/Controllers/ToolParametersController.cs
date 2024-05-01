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
    [Route("toolparameters")]
    public class ToolParametersController
	{
        private readonly ILogger<ToolKeyParam> _logger;
        private readonly WordDbContext _context;

        public ToolParametersController(WordDbContext context, ILogger<ToolKeyParam> logger)
		{
            _context = context;
            _logger = logger;
        }

        [HttpGet("get/{category}/{key}")]
        public async Task<ActionResult<ToolKeyParam>> GetToolParameters(string category, string key)
        {
            ToolKeyParam p = await _context.GetToolKeyParam(category, key);
            if (p == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(p);
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<OperationResult<ToolKeyParam>>> AddToolParameters(ToolKeyParam param)
        {
            OperationResult<ToolKeyParam> or = await _context.AddToolKeyParam(param, _logger);
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

        [HttpPut("update")]
        public async Task<ActionResult<OperationResult<ToolKeyParam>>> UpdateToolParameters(ToolKeyParam param)
        {
            OperationResult<ToolKeyParam> or = await _context.UpdateToolKeyParam(param, _logger);
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

        [HttpPut("Delete")]
        public async Task<ActionResult<OperationResult<ToolKeyParam>>> DeleteToolParameters(ToolKeyParam param)
        {
            OperationResult<ToolKeyParam> or = await _context.DeleteToolKeyParam(param, _logger);
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
    }
}

