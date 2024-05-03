using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Dictionary.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dictionary.Controllers
{
    [ApiController]
    [Route("oplog")]
    public class OpLogController : Controller
    {
        private readonly ILogger<EnergyDataController> _logger;
        private readonly WordDbContext _context;

        public OpLogController(WordDbContext context, ILogger<EnergyDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("logs/{key}/{startDate}/{endDate}")]
        public ActionResult GetLogs(string key, DateTime startDate, DateTime endDate)
        {
            endDate = endDate.AddDays(1);
            List<OpLog> s = _context.OpLogs.Where(o => o.Key.ToLower() == key.ToLower() && o.LogTime >= startDate && o.LogTime <= endDate)
                .OrderBy(o => o.Id)
                .ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpPost("addlog")]
        public ActionResult AddLog(OpLog opLog)
        {
            try
            {
                _context.OpLogs.Add(opLog);
                _context.SaveChanges();
                return new OkObjectResult(opLog);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"AddLog failed, e:{e.Message}");
                return new BadRequestObjectResult(opLog);
            }
        }

        [HttpPost("addlogs")]
        public ActionResult AddLogs(List<OpLog> opLogs)
        {
            try
            {
                _context.OpLogs.AddRange(opLogs);
                _context.SaveChanges();
                return new OkObjectResult(opLogs);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"AddLogs failed, e:{e.Message}");
                return new BadRequestObjectResult(opLogs);
            }
        }

        [HttpDelete("deletelog/{id}")]
        public ActionResult DeleteLog(int id)
        {
            try
            {
                var opLog = _context.OpLogs.Where(o => o.Id == id).First();
                _context.OpLogs.Remove(opLog);
                _context.SaveChanges();
                return new OkObjectResult(opLog);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"DeleteLog failed (id: {id}), e:{e.Message}");
                return new BadRequestObjectResult(id);
            }
        }

        [HttpDelete("deletelogs/{key}/{startDate}/{endDate}")]
        public ActionResult DeleteLogs(string key, DateTime startDate, DateTime endDate)
        {
            try
            {
                endDate = endDate.AddDays(1);
                var opLogs = _context.OpLogs.Where(o => o.Key.ToLower() == key.ToLower() && o.LogTime >= startDate && o.LogTime <= endDate);
                _context.OpLogs.RemoveRange(opLogs);
                _context.SaveChanges();
                return new OkObjectResult(opLogs);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"DeleteLog failed (key: {key}, startDate: {startDate}, endDate: {endDate}), e:{e.Message}");
                return new BadRequestObjectResult(new string[] { key, startDate.ToString(), endDate.ToString() });
            }
        }
    }
}

