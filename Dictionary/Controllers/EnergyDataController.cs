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


        [HttpGet("gettype")]
        public ActionResult GetEnergyType()
        {
            List<ConsumerGoods> s = _context.ConsumerGoods.ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
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

        [HttpGet("getlatest/{energyType}")]
        public ActionResult GetLatestEnergyData(int energyType)
        {
            ConsumerGoodsDetail s = _context.GetEnergyDataLatest(energyType);
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
        public async Task<ActionResult<ConsumerGoodsDetail>> AddEnergyData(ConsumerGoodsDetail energyData)
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
        public async Task<ActionResult<OperationResult<ConsumerGoodsDetail>>> AddEnergyData(List<ConsumerGoodsDetail> energyDatas)
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

        [HttpPut("delete/{id}")]
        public async Task<ActionResult<string>> DeleteEnergyData(int id)
        {
            try
            {
                var detail = await _context.ConsumerGoodsDetails.FirstOrDefaultAsync(d => d.Id == id);
                if (detail != null)
                {
                    _context.ConsumerGoodsDetails.Remove(detail);
                    _context.SaveChanges();
                    return new OkObjectResult(string.Empty);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult($"Exception raised on deleting energy data with id={id}. Exception message is {e.Message}.");
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<ConsumerGoodsDetail>> UpdateEnergyData(ConsumerGoodsDetail detail)
        {
            try
            {
                var dbDetail = await _context.ConsumerGoodsDetails.FirstOrDefaultAsync(d => d.Id == detail.Id);
                if (dbDetail != null)
                {
                    dbDetail.CopyFrom(detail);
                    dbDetail.Info = "";
                    _context.SaveChanges();
                    await _context.Entry(dbDetail).ReloadAsync();

                    return new OkObjectResult(dbDetail);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult($"Exception raised on updating energy data with id={detail.Id}. Exception message is {e.Message}.");
            }
        }

        [HttpGet("getenergyparams")]
        public ActionResult GetEnergyParameters()
        {
            List<ConsumerGoodsParameters> consumerGoodsParameters = _context.ConsumerGoodsParameters.Where(p => p.Valid == true).OrderBy(p => p.ConsumerGoodsId).ToList();
            if (consumerGoodsParameters != null && consumerGoodsParameters.Count == 2)
            {
                int quantity, price;
                float trans;
                ConsumerGoodsParameters electric = consumerGoodsParameters.FirstOrDefault(p => p.ConsumerGoodsId == 1);
                string[] parameters = electric.Params.Split(',');
                quantity = Int32.Parse(parameters[0]);
                price = Int32.Parse(parameters[1]);
                EnergyParam electricParam = new EnergyParam()
                {
                    ConsumerGoodsId = electric.ConsumerGoodsId,
                    StartDay = electric.StartDay,
                    Quantity = quantity,
                    Price = price
                };
                ConsumerGoodsParameters gas = consumerGoodsParameters.FirstOrDefault(p => p.ConsumerGoodsId == 2);
                parameters = gas.Params.Split(',');
                quantity = Int32.Parse(parameters[0]);
                price = Int32.Parse(parameters[1]);
                trans = float.Parse(parameters[2]);
                GasParam gasParam = new GasParam()
                {
                    ConsumerGoodsId = gas.ConsumerGoodsId,
                    StartDay = gas.StartDay,
                    Quantity = quantity,
                    Price = price,
                    Trans = trans
                };

                return new OkObjectResult(new Tuple<EnergyParam, GasParam>(electricParam, gasParam));
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpPut("updateenergyparams")]
        public async Task<ActionResult<string>> UpdateEnergyParameters(AllEnergyParams energyParameters)
        {
            string or = await _context.UpdateEnergyParameters(energyParameters, _logger);
            if (string.IsNullOrEmpty(or))
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

