using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dictionary.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dictionary.Controllers
{
    [ApiController]
    [Route("energydata")]
    public class EnergyDataController : Controller
    {
        private readonly ILogger<EnergyDataController> _logger;
        private readonly WordDbContext _context;

        public EnergyDataController(WordDbContext context, ILogger<EnergyDataController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet("get/{energyType}/{year}/{month}")]
        public ActionResult GetEnergyData(int energyType, int year, int month)
        {
            List<ConsumerGoodsDetail> s = _context.GetEnergyData(energyType, year, month);
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<ConsumerGoodsDetail>> PostWord(ConsumerGoodsDetail energyData)
        {
            ConsumerGoodsDetail or = await _context.AddEnergyData(energyData, _logger);
            if (!(or is null))
            {
                return new OkObjectResult(or);
            }
            else
            {
                return new BadRequestObjectResult(or);
            }
        }

        [HttpPost("batchadd")]
        public async Task<ActionResult<OperationResult<ConsumerGoodsDetail>>> PostWord(List<ConsumerGoodsDetail> energyDatas)
        {
            OperationResult<ConsumerGoodsDetail> or = new OperationResult<ConsumerGoodsDetail>();

            foreach (var energyData in energyDatas)
            {
                ConsumerGoodsDetail cgd = await _context.AddEnergyData(energyData, _logger);
                if (!(cgd is null))
                {
                    or.Oks.Add(cgd);
                }
                else
                {
                    or.UnknownErrors.Add(energyData);
                }
            }

            return or;
        }
    }
}

