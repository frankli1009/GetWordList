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
        public ActionResult GetDailyTaskTypes()
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

        [HttpGet("themes")]
        public ActionResult GetDailyTaskThemes()
        {
            List<DailyTaskTheme> s = _context.DailyTaskThemes.ToList();
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
        public ActionResult GetDailyTaskStatuses()
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
            var s = _context.DailyTasks.Where(d => d.DailyTaskStatusId < 3 && d.Suspended == 0)
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

        [HttpGet("gettasksoverdue")]
        public ActionResult GetDailyTasksOverdue()
        {
            string sql = "Select * from DailyTasks where " +
                "((DailyTaskTypeId in (1, 2) and Id in (" +
                "select DailyTaskId from DailyTaskSchedules where Id in ("+
                "select a.DailyTaskScheduleId from DailyTaskScheduleDetails a,  "+
                "DailyTaskSubs b where a.DailyTaskStatusId<3 and b.Optional=0 "+
                "and a.DailyTaskSubId=b.Id) and ActDate<Convert(DateTime, '" +
                DateTime.Now.ToString("yyyy-MM-dd") + "', 20))) " +
                "or " +
                "(DailyTaskTypeId = 3 and EndDate<Convert(DateTime, '" +
                DateTime.Now.ToString("yyyy-MM-dd") + "', 20))) "+
                "and DailyTaskStatusId < 3 and Suspended = 0";
                //"Id in ("+
                //"Select DailyTaskId from DailyTaskSchedules where Id in (" +
                //"select DailyTaskScheduleId from DailyTaskScheduleDetails where " +
                //"DailyTaskStatusId < 3)))";
            var s = _context.DailyTasks.FromSqlRaw(sql)
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

        [HttpGet("gettaskshist")]
        public async Task<ActionResult> GetDailyTasksHist(QueryConds queryConds)
        {
            IQueryable<DailyTask> s = null;
            if (queryConds.QueryType == (int)QueryType.ByDate)
            {
                int dateType = Int32.Parse(queryConds.QueryParams[1]);
                DateTime start = DateTime.Parse(queryConds.QueryParams[2]);
                DateTime end = DateTime.Parse(queryConds.QueryParams[3]).AddDays(1);
                if (dateType == (int)QueryDateType.ByStartDate) // StartDate
                {
                    s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.StartDate < end && d.StartDate >= start);
                }
                else if (dateType == (int)QueryDateType.ByEndDate) // EndDate
                {
                    s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.EndDate < end && d.EndDate >= start);
                }
                else if (dateType == (int)QueryDateType.ByCreateTime) // Time
                {
                    s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.Time < end && d.Time >= start);
                }
                else if (dateType == (int)QueryDateType.ByDoneTime) // DoneTime
                {
                    s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.DoneTime < end && d.DoneTime >= start);
                }
                else // StartDate >= EndDate <=
                {
                    s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.EndDate < end && d.StartDate >= start);
                }
            }
            else if (queryConds.QueryType == (int)QueryType.ById)
            {
                int taskId = Int32.Parse(queryConds.QueryParams[1]);
                s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.Id == taskId);
            }
            else if (queryConds.QueryType == (int)QueryType.ByName)
            {
                string taskName = queryConds.QueryParams[1];
                s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.Name.IndexOf(taskName) > -1);
            }
            else if (queryConds.QueryType == (int)QueryType.ByCombination)
            {
                int taskYear = Int32.Parse(queryConds.QueryParams[1]);
                string taskName = queryConds.QueryParams[2];
                s = _context.DailyTasks.Where(d => d.StartDate.Year <= taskYear && d.EndDate.Year >= taskYear);
                if (!string.IsNullOrWhiteSpace(taskName)) s = s.Where(d => d.Name.IndexOf(taskName) > -1);
            }

            return await GetQueryResult(queryConds, s, true);
        }

        private async Task<T> GetParameter<T>(string category, string key, T defaultValue)
        {

            ToolKeyParam tkpCountPerPage = await _context.GetToolKeyParam(category, key);
            if (tkpCountPerPage != null)
            {
                try
                {
                    return (T)Convert.ChangeType(tkpCountPerPage.Parameters, typeof(T)); ;
                }
                catch
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }

        private async Task<ActionResult> GetQueryResult(QueryConds queryConds, IQueryable<DailyTask> s, bool historyQuery)
        {
            int queryTheme = Int32.Parse(queryConds.QueryParams[0]);
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                if (queryTheme > 0) s = s.Where(t => t.DailyTaskThemeId == queryTheme);
                int totalCount = s.Count();
                if (totalCount > 0)
                {
                    int countPerPage = await GetParameter<int>("DailyTask",
                        historyQuery ? "History.CountPerPage" : "Management.CountPerPage",
                        20);

                    PageInfo pageInfo = new()
                    {
                        CountPerPage = countPerPage,
                        CurrentPage = queryConds.QueryPage,
                        TotalCount = totalCount,
                        TotalPageCount = totalCount / countPerPage + (totalCount % countPerPage == 0 ? 0 : 1)
                    };
                    List<DailyTask> ls;
                    s = s.OrderByDescending(d => d.Id);

                    int minimumCountToSplitIntoPages = await GetParameter<int>("DailyTask",
                        historyQuery ? "History.MinimumCountToSplitIntoPages" : "Management.MinimumCountToSplitIntoPages",
                        30); 
                    if (totalCount > minimumCountToSplitIntoPages)
                    {
                        if (totalCount > countPerPage * (queryConds.QueryPage - 1))
                        {
                            if (totalCount <= countPerPage * queryConds.QueryPage)
                            {
                                s = s.Skip(countPerPage * (queryConds.QueryPage - 1));
                            }
                            else
                            {
                                s = s.Skip(countPerPage * (queryConds.QueryPage - 1)).Take(countPerPage);
                            }
                            if (queryConds.QueryPage > 1)
                            {
                                pageInfo.FirstPage = true;
                                pageInfo.PrevPage = true;
                                pageInfo.Prev1Page = queryConds.QueryPage - 1;
                            }
                            if (queryConds.QueryPage > 2) pageInfo.Prev2Page = queryConds.QueryPage - 2;
                            if (queryConds.QueryPage + 1 <= pageInfo.TotalPageCount)
                            {
                                pageInfo.NextPage = true;
                                pageInfo.LastPage = true;
                                pageInfo.Next1Page = queryConds.QueryPage + 1;
                            }
                            if (queryConds.QueryPage + 2 <= pageInfo.TotalPageCount) pageInfo.Next2Page = queryConds.QueryPage + 2;
                        }
                        else
                        {
                            return new NotFoundResult();
                        }
                    }
                    else
                    {
                        pageInfo.CountPerPage = minimumCountToSplitIntoPages;
                        pageInfo.TotalPageCount = 1;
                    }
                    ls = s.ToList();
                    PageData<DailyTask> pageData = new()
                    {
                        Page = pageInfo,
                        Data = ls
                    };

                    return new OkObjectResult(pageData);
                }
                else
                {
                    return new NotFoundResult();
                }
            }

        }

        [HttpGet("gettasks")]
        public async Task<ActionResult> GetDailyTasks(QueryConds queryConds)
        {
            IQueryable<DailyTask> s = null;
            int queryTheme = Int32.Parse(queryConds.QueryParams[0]);
            if (queryConds.QueryType == (int)QueryType.ByDate)
            {
                int dateType = Int32.Parse(queryConds.QueryParams[1]);
                DateTime start = DateTime.Parse(queryConds.QueryParams[2]);
                DateTime end = DateTime.Parse(queryConds.QueryParams[3]).AddDays(1);
                if (dateType == (int)QueryDateType.ByStartDate) // StartDate
                {
                    s = _context.DailyTasks.Where(d => d.StartDate < end && d.StartDate >= start);
                }
                else if (dateType == (int)QueryDateType.ByEndDate) // EndDate
                {
                    s = _context.DailyTasks.Where(d => d.EndDate < end && d.EndDate >= start);
                }
                else if (dateType == (int)QueryDateType.ByCreateTime) // Time
                {
                    s = _context.DailyTasks.Where(d => d.Time < end && d.Time >= start);
                }
                else if (dateType == (int)QueryDateType.ByDoneTime) // DoneTime
                {
                    s = _context.DailyTasks.Where(d => d.DoneTime < end && d.DoneTime >= start);
                }
                else // StartDate >= EndDate <=
                {
                    s = _context.DailyTasks.Where(d => d.EndDate < end && d.StartDate >= start);
                }
            }
            else if (queryConds.QueryType == (int)QueryType.ById)
            {
                int taskId = Int32.Parse(queryConds.QueryParams[1]);
                s = _context.DailyTasks.Where(d => d.Id == taskId);
            }
            else if (queryConds.QueryType == (int)QueryType.ByName)
            {
                string taskName = queryConds.QueryParams[1];
                s = _context.DailyTasks.Where(d => d.Name.IndexOf(taskName) > -1);
            }
            else if (queryConds.QueryType == (int)QueryType.ByCombination)
            {
                int taskYear = Int32.Parse(queryConds.QueryParams[1]);
                string taskName = queryConds.QueryParams[2];
                s = _context.DailyTasks.Where(d => d.StartDate.Year <= taskYear && d.EndDate.Year >= taskYear);
                if (!string.IsNullOrWhiteSpace(taskName)) s = s.Where(d => d.Name.IndexOf(taskName) > -1);
            }
            return await GetQueryResult(queryConds, s, false);
        }

        [HttpGet("getmoretaskstoday")]
        public ActionResult GetDailyTasksMore()
        {
            var s = _context.DailyTasks.Where(d => d.DailyTaskStatusId == 3 && d.Suspended == 0 &&
                    ((d.EndDate.Year == DateTime.Now.Year && d.EndDate.DayOfYear >= DateTime.Now.DayOfYear) ||
                    (d.EndDate.Year > DateTime.Now.Year)))
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

        [HttpGet("gettasksubs/{taskid}")]
        public ActionResult GetDailyTaskSubs(int taskId)
        {
            List<DailyTaskSubUnit> sus = new List<DailyTaskSubUnit>();
            List<DailyTaskSub> s = _context.DailyTaskSubs.Where(t => t.DailyTaskId == taskId).OrderBy(t => t.OrderId).ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                foreach (var s1 in s)
                {
                    DailyTaskSubUnit su = new DailyTaskSubUnit() { DailyTaskSub = s1 };
                    List<DailyTaskSubExtraInfo> sei = _context.DailyTaskSubExtraInfos
                        .Where(ei => ei.DailyTaskId == taskId && ei.DailyTaskSubId == s1.Id)
                        .OrderBy(ei => ei.OrderId)
                        .ToList();
                    su.DailyTaskSubExtraInfos.AddRange(sei);
                    sus.Add(su);
                }
                return new OkObjectResult(sus);
            }
        }

        [HttpGet("gettaskschedules/{taskid}")]
        public ActionResult GetDailyTaskSchedules(int taskId)
        {
            List<DailyTaskScheduleUnit> sus = new List<DailyTaskScheduleUnit>();
            List<DailyTaskSchedule> s = _context.DailyTaskSchedules.Where(t => t.DailyTaskId == taskId).OrderBy(t => t.ActDate).ToList();
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
                        .OrderBy(d => d.OrderId)
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

        [HttpPost("update")]
        public async Task<ActionResult<DailyTaskResponse>> UpdateDailyTask(DailyTaskSubmit dailyTaskSubmit)
        {
            DailyTaskResponse dailyTaskResponse = await _context.UpdateDailyTask(dailyTaskSubmit, _logger);
            if (!(dailyTaskResponse is null) && (dailyTaskResponse.Errors.Count == 0))
            {
                return new OkObjectResult(dailyTaskResponse);
            }
            else
            {
                return new BadRequestObjectResult(dailyTaskResponse);
            }
        }

        [HttpPost("updateschedule")]
        public async Task<ActionResult<DailyTaskScheduleResponse>> UpdateDailyTaskDetails(DailyTaskScheduleSubmit dailyTaskScheduleSubmit)
        {
            DailyTaskScheduleResponse dailyTaskScheduleResponse = await _context.UpdateDailyTaskSchedules(dailyTaskScheduleSubmit, _logger);
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
                int taskStatusId = task.DailyTaskStatusId;
                return new OkObjectResult(new Tuple<int, int>(statusId, taskStatusId));
            }
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<string>> DeleteDailyTask(DailyTaskSuspended dtSuspended)
        {
            var t = _context.DailyTasks.Where(d => d.Id == dtSuspended.DailyTaskId).FirstOrDefault();
            if (t == null)
            {
                return new BadRequestObjectResult($"Task {dtSuspended.DailyTaskId} does not exist.");
            }
            else
            {
                try
                {
                    if (t.Suspended < 2)
                    {
                        t.Suspended = 2;

                        if (_context.DailyTaskSuspendeds.Any(s => s.DailyTaskId == dtSuspended.DailyTaskId))
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                                "UPDATE [dbo].[DailyTaskSuspendeds] SET SuspendedId = SuspendedId + 1 WHERE DailyTaskId = " +
                                dtSuspended.DailyTaskId.ToString());
                        }
                        await _context.DailyTaskSuspendeds.AddAsync(dtSuspended);

                        await _context.SaveChangesAsync();

                        return new OkObjectResult("Delete DailyTask OK.");
                    }
                    else
                    {
                        // undelete the task
                        DailyTaskSuspended s = _context.DailyTaskSuspendeds.Where(d2 => d2.DailyTaskId == dtSuspended.DailyTaskId && d2.SuspendedId == 0).FirstOrDefault();
                        if (s == null)
                        {
                            return new BadRequestObjectResult($"Original deleted record of task {dtSuspended.DailyTaskId} does not exist.");
                        }
                        else
                        {
                            s.Info = s.Info + "; " + dtSuspended.Info;
                            s.EndDate = dtSuspended.EndDate;

                            // renew the task schedule dates
                            DateTime endDate = s.EndDate ?? s.StartDate;
                            int suspendDays = (endDate - s.StartDate).Days;
                            await _context.Database.ExecuteSqlRawAsync(
                                "UPDATE [dbo].[DailyTaskSchedules] SET ActDate = DATEADD(dd, " + suspendDays.ToString() +
                                ", ActDate) WHERE DailyTaskId = " + dtSuspended.DailyTaskId.ToString() +
                                " AND DailyTaskStatusId = 1 AND ActDate >= CONVERT(DATETIME, '" +
                                s.StartDate.ToString("yyyy-MM-dd") + "', 102)");

                            t.Suspended = 0;
                            t.EndDate = t.EndDate.AddDays(suspendDays);

                            await _context.SaveChangesAsync();

                            return new OkObjectResult("Undelete DailyTask OK.");
                        }
                    }
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e.Message);
                }
            }
        }

        [HttpPut("suspended")]
        public async Task<ActionResult<string>> SuspendedDailyTask(DailyTaskSuspended dtSuspended)
        {
            DailyTask t = _context.DailyTasks.Where(d => d.Id == dtSuspended.DailyTaskId).FirstOrDefault();
            if (t == null)
            {
                return new BadRequestObjectResult($"Task {dtSuspended.DailyTaskId} does not exist.");
            }
            else
            {
                try
                {
                    if (t.Suspended == 1)
                    {
                        // reactivate the task
                        DailyTaskSuspended s = _context.DailyTaskSuspendeds.Where(d2 => d2.DailyTaskId == dtSuspended.DailyTaskId && d2.SuspendedId == 0).FirstOrDefault();
                        if (s == null)
                        {
                            return new BadRequestObjectResult($"Original suspended record of task {dtSuspended.DailyTaskId} does not exist.");
                        }
                        else
                        {
                            s.Info = s.Info + "; " + dtSuspended.Info;
                            s.EndDate = dtSuspended.EndDate;

                            // renew the task schedule dates
                            DateTime endDate = s.EndDate ?? s.StartDate;
                            int suspendDays = (endDate - s.StartDate).Days;
                            await _context.Database.ExecuteSqlRawAsync(
                                "UPDATE [dbo].[DailyTaskSchedules] SET ActDate = DATEADD(dd, " + suspendDays.ToString() +
                                ", ActDate) WHERE DailyTaskId = " + dtSuspended.DailyTaskId.ToString() +
                                " AND DailyTaskStatusId = 1 AND ActDate >= CONVERT(DATETIME, '" +
                                s.StartDate.ToString("yyyy-MM-dd") + "', 102)");

                            t.Suspended = 0;
                            t.EndDate = t.EndDate.AddDays(suspendDays);

                            await _context.SaveChangesAsync();

                            return new OkObjectResult("Activate suspended DailyTask OK.");
                        }
                    }
                    else
                    {
                        t.Suspended = 1;

                        if (_context.DailyTaskSuspendeds.Any(s => s.DailyTaskId == dtSuspended.DailyTaskId))
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                                "UPDATE [dbo].[DailyTaskSuspendeds] SET SuspendedId = SuspendedId + 1 WHERE DailyTaskId = " +
                                dtSuspended.DailyTaskId.ToString());
                        }
                        await _context.DailyTaskSuspendeds.AddAsync(dtSuspended);

                        await _context.SaveChangesAsync();

                        return new OkObjectResult("Suspend DailyTask OK.");
                    }

                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e.Message);
                }
            }
        }

        [HttpGet("getsuspendeds/{taskid}")]
        public ActionResult GetDailyTaskSuspendeds(int taskId)
        {
            List<DailyTaskSuspended> s = _context.DailyTaskSuspendeds.Where(t => t.DailyTaskId == taskId).OrderBy(t => t.SuspendedId).ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }
    }
}

