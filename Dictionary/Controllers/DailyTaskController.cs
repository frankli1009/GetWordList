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
    [Route("dailytask")]
    public class DailyTaskController : Controller
    {
        private readonly ILogger<EnergyDataController> _logger;
        private readonly WordDbContext _context;

        public DailyTaskController(WordDbContext context, ILogger<EnergyDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("types")]
        public ActionResult GetDailyTaskType()
        {
            List<DailyTaskType> s = _context.DailyTaskTypes.ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpGet("statuses")]
        public ActionResult GetDailyTaskStatus()
        {
            List<DailyTaskStatus> s = _context.DailyTaskStatuses.ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpGet("gettask/{id}")]
        public ActionResult GetDailyTask(int id)
        {
            DailyTask s = _context.DailyTasks.Where(t => t.Id == id).FirstOrDefault();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpGet("gettaskscur")]
        public ActionResult GetDailyTasksCur()
        {
            var s = _context.DailyTasks.Where(d => d.DailyTaskStatusId < 3)
                .OrderBy(d => d.Id)
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

        [HttpGet("gettaskshist/{datetype}/{startdate}/{enddate}")]
        public ActionResult GetDailyTasksHist(int dateType, string startDate, string endDate)
        {
            DateTime start = DateTime.Parse(startDate);
            DateTime end = DateTime.Parse(endDate).AddDays(1);
            List<DailyTask> s;
            if (dateType == 1) // StartDate
            {
                s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.StartDate < end && d.StartDate >= start)
                .OrderBy(d => d.Id)
                .ToList();
            }
            else if (dateType == 2) // EndDate
            {
                s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.EndDate < end && d.EndDate >= start)
                .OrderBy(d => d.Id)
                .ToList();
            }
            else // StartDate >= EndDate <=
            {
                s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.EndDate < end && d.StartDate >= start)
                .OrderBy(d => d.Id)
                .ToList();
            }
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpGet("gettasksubs/{taskid}")]
        public ActionResult GetDailyTaskSub(int taskId)
        {
            List<DailyTaskSub> s = _context.DailyTaskSubs.Where(t => t.DailyTaskId == taskId).OrderBy(t => t.Id).ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpGet("gettaskschedules/{taskid}")]
        public ActionResult GetDailyTaskSchedules(int taskId)
        {
            List<DailyTaskScheduleUnit> sus = new List<DailyTaskScheduleUnit>();
            List<DailyTaskSchedule> s = _context.DailyTaskSchedules.Where(t => t.DailyTaskId == taskId).OrderBy(t => t.Id).ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                foreach (var s1 in s)
                {
                    DailyTaskScheduleUnit su = new DailyTaskScheduleUnit() { DailyTaskSchedule = s1 };
                    List<DailyTaskScheduleDetail> sd = _context.DailyTaskScheduleDetails
                        .Where(d => d.DailyTaskScheduleId == su.DailyTaskSchedule.Id)
                        .OrderBy(d => d.Id)
                        .ToList();
                    su.DailyTaskScheduleDetails.AddRange(sd);
                    sus.Add(su);
                }
                return new OkObjectResult(sus);
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<DailyTaskResponse>> AddDailyTask(DailyTaskSubmit dailyTaskSubmit)
        {
            DailyTaskResponse dailyTaskResponse = await _context.AddDailyTask(dailyTaskSubmit, _logger);
            if (!(dailyTaskResponse is null) && (dailyTaskResponse.Errors.Count == 0))
            {
                return new OkObjectResult(dailyTaskResponse);
            }
            else
            {
                return new BadRequestObjectResult(dailyTaskResponse);
            }
        }

        [HttpPost("adddschedule")]
        public async Task<ActionResult<DailyTaskScheduleResponse>> AddDailyTaskDetails(DailyTaskScheduleSubmit dailyTaskScheduleSubmit)
        {
            DailyTaskScheduleResponse dailyTaskScheduleResponse = await _context.AddDailyTaskSchedules(dailyTaskScheduleSubmit, _logger);
            if (!(dailyTaskScheduleResponse is null) && dailyTaskScheduleResponse.Errors.Count == 0)
            {
                return new OkObjectResult(dailyTaskScheduleResponse);
            }
            else
            {
                return new BadRequestObjectResult(dailyTaskScheduleResponse);
            }
        }

        [HttpPut("updatestatus/{id}/{statusId}")]
        public async Task<ActionResult<Tuple<int, int>>> UpdateScheduleDetailStatus(int Id, int statusId)
        {
            var s = _context.DailyTaskScheduleDetails.Where(d => d.Id == Id).FirstOrDefault();
            if (s == null)
            {
                return new BadRequestObjectResult(new Tuple<int, int>(0, 0));
            }
            else
            {
                s.DailyTaskStatusId = statusId;
                _context.SaveChanges();

                var sub = _context.DailyTaskSubs.Where(su => su.Id == s.DailyTaskSubId).First();
                var task = _context.DailyTasks.Where(t => t.Id == sub.DailyTaskId).First();
                if (task.DailyTaskStatusId == statusId)
                {
                    return new OkObjectResult(new Tuple<int, int>(statusId, statusId));
                }

                var schedules = _context.DailyTaskSchedules.Where(sc => sc.DailyTaskId == task.Id).ToList();
                if (statusId == 1)
                {
                    for(int i = 0; i < schedules.Count(); i++)
                    {
                        if(_context.DailyTaskScheduleDetails.Any(d => d.DailyTaskScheduleId == schedules[i].Id && d.DailyTaskStatusId > 1))
                        {
                            if (task.DailyTaskStatusId == 3)
                            {
                                task.DailyTaskStatusId = 2;
                                _context.SaveChanges();
                            }
                            return new OkObjectResult(new Tuple<int, int>(1, 2));
                        }
                    }
                    return new OkObjectResult(new Tuple<int, int>(1, 1));
                }
                else //if (statusId == 3) // only 1 and 3 for sub task status
                {
                    for (int i = 0; i < schedules.Count(); i++)
                    {
                        if (_context.DailyTaskScheduleDetails.Any(d => d.DailyTaskScheduleId == schedules[i].Id && d.DailyTaskStatusId < 3))
                        {
                            if (task.DailyTaskStatusId == 1)
                            {
                                task.DailyTaskStatusId = 2;
                                _context.SaveChanges();
                            }
                            return new OkObjectResult(new Tuple<int, int>(3, 2));
                        }
                    }
                    task.DailyTaskStatusId = 3;
                    _context.SaveChanges();
                    return new OkObjectResult(new Tuple<int, int>(3, 3));
                }
            }
        }
    }
}

