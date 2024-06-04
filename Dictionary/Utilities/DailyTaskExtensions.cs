using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Migrations;
using Dictionary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dictionary.Utilities
{
	public static class DailyTaskExtensions
	{
        public static DailyTask GetDailyTask(this WordDbContext context, int Id)
        {
            return context.DailyTasks.Where(s => s.Id == Id).First();
        }

        public static List<DailyTaskSub> GetDailyTaskSubs(this WordDbContext context, int taskId)
        {
            return context.DailyTaskSubs.Where(s => s.DailyTaskId == taskId).OrderBy(s => s.OrderId).ToList();
        }

        public static async Task<DailyTaskResponse> AddDailyTask(this WordDbContext context, DailyTaskSubmit dailyTaskSubmit, ILogger logger)
        {
            DailyTaskResponse dailyTaskResponse = new DailyTaskResponse();
            dailyTaskResponse.DailyTaskSubmit = dailyTaskSubmit;
            try
            {
                var dailyTaskEntry = await context.DailyTasks.AddAsync(dailyTaskSubmit.DailyTask);
                await context.SaveChangesAsync();

                foreach (var sub in dailyTaskSubmit.DailyTaskSubs)
                {
                    sub.DailyTaskId = dailyTaskSubmit.DailyTask.Id;
                    await context.DailyTaskSubs.AddAsync(sub);
                    await context.SaveChangesAsync();
                }

                return dailyTaskResponse;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", dailyTaskSubmit);
                dailyTaskResponse.Errors.Add(e.Message);
                Exception e1 = e.InnerException;
                int maxErrorMessages = 50;
                int indexErrorMessage = 0;
                while (e1 != null && indexErrorMessage < maxErrorMessages)
                {
                    dailyTaskResponse.Errors.Add(e1.Message);
                    e1 = e1.InnerException;
                    indexErrorMessage++;
                }
                return dailyTaskResponse;
            }
        }

        public static async Task<DailyTask> AddDailyTask(this WordDbContext context, DailyTask data, ILogger logger)
        {
            try
            {
                await context.DailyTasks.AddAsync(data);
                await context.SaveChangesAsync();
                return data;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", data);
                return null;
            }
        }

        public static async Task<DailyTaskSub> AddDailyTaskSub(this WordDbContext context, DailyTaskSub data, ILogger logger)
        {
            try
            {
                await context.DailyTaskSubs.AddAsync(data);
                await context.SaveChangesAsync();
                return data;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", data);
                return null;
            }
        }

        public static async Task<DailyTaskSchedule> AddDailyTaskSchedule(this WordDbContext context, DailyTaskSchedule data, ILogger logger)
        {
            try
            {
                await context.DailyTaskSchedules.AddAsync(data);
                await context.SaveChangesAsync();
                return data;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", data);
                return null;
            }
        }

        public static void CopyFrom(this DailyTask dailyTask, DailyTask src)
        {
            dailyTask.DailyTaskStatusId = src.DailyTaskStatusId;
            dailyTask.DailyTaskTypeId = src.DailyTaskTypeId;
            dailyTask.EndDate = src.EndDate;
            dailyTask.StartDate = src.StartDate;
            dailyTask.Info = src.Info;
            dailyTask.Name = src.Name;
            dailyTask.Time = src.Time;
        }

        public static void CopyFrom(this DailyTaskSub taskSub, DailyTaskSub src)
        {
            taskSub.Info = src.Info;
            taskSub.WorkLoad = src.WorkLoad;
            taskSub.OrderId = src.OrderId;
            taskSub.ExtraInfo = src.ExtraInfo;
        }

        public static async Task<DailyTaskResponse> UpdateDailyTask(this WordDbContext context, DailyTaskSubmit dailyTaskSubmit, ILogger logger)
        {
            DailyTaskResponse dailyTaskResponse = new DailyTaskResponse();
            dailyTaskResponse.DailyTaskSubmit = dailyTaskSubmit;
            try
            {
                var dailyTask = context.DailyTasks.FirstOrDefault(t => t.Id == dailyTaskSubmit.DailyTask.Id);
                if (dailyTask != null)
                {
                    dailyTask.CopyFrom(dailyTaskSubmit.DailyTask);
                    await context.SaveChangesAsync();

                    foreach (var sub in dailyTaskSubmit.DailyTaskSubs)
                    {
                        if (sub.Id > 0)
                        {
                            // Edited subs
                            var dailyTaskSub = context.DailyTaskSubs.FirstOrDefault(s => s.Id == sub.Id);
                            if (dailyTaskSub != null)
                            {
                                dailyTaskSub.CopyFrom(sub);
                                await context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            // New subs
                            sub.DailyTaskId = dailyTaskSubmit.DailyTask.Id;
                            await context.DailyTaskSubs.AddAsync(sub);
                            await context.SaveChangesAsync();
                        }
                    }

                    // Deleted subs
                    var newAllSubs = context.DailyTaskSubs.Where(s => s.DailyTaskId == dailyTask.Id).ToList();
                    var toDelete = newAllSubs.Where(s => !dailyTaskSubmit.DailyTaskSubs.Any(s2 => s2.DailyTaskId == dailyTask.Id && s2.Id == s.Id));
                    if (toDelete.Count() > 0)
                    {
                        context.DailyTaskSubs.RemoveRange(toDelete);
                        await context.SaveChangesAsync();
                    }
                }


                return dailyTaskResponse;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to update/{0}", dailyTaskSubmit);
                dailyTaskResponse.Errors.Add(e.Message);
                Exception e1 = e.InnerException;
                int maxErrorMessages = 50;
                int indexErrorMessage = 0;
                while (e1 != null && indexErrorMessage < maxErrorMessages)
                {
                    dailyTaskResponse.Errors.Add(e1.Message);
                    e1 = e1.InnerException;
                    indexErrorMessage++;
                }
                return dailyTaskResponse;
            }
        }

        public static async Task<DailyTaskScheduleDetail> AddDailyTaskScheduleDetail(this WordDbContext context, DailyTaskScheduleDetail data, ILogger logger)
        {
            try
            {
                await context.DailyTaskScheduleDetails.AddAsync(data);
                await context.SaveChangesAsync();
                return data;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", data);
                return null;
            }
        }

        public static async Task<DailyTaskScheduleResponse> AddDailyTaskSchedules(this WordDbContext context, DailyTaskScheduleSubmit data, ILogger logger)
        {
            DailyTaskScheduleResponse dailyTaskScheduleResponse = new DailyTaskScheduleResponse();
            dailyTaskScheduleResponse.DailyTaskScheduleSubmit = data;
            try
            {
                foreach (var scheduleUnit in data.DailyTaskScheduleUnits)
                {
                    await context.DailyTaskSchedules.AddAsync(scheduleUnit.DailyTaskSchedule);
                    await context.SaveChangesAsync();

                    foreach (var detail in scheduleUnit.DailyTaskScheduleDetails)
                    {
                        detail.DailyTaskScheduleId = scheduleUnit.DailyTaskSchedule.Id;
                        await context.DailyTaskScheduleDetails.AddAsync(detail);
                        await context.SaveChangesAsync();
                    }
                }
                return dailyTaskScheduleResponse;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", data);
                dailyTaskScheduleResponse.Errors.Add(e.Message);
                Exception e1 = e.InnerException;
                int maxErrorMessages = 50;
                int indexErrorMessage = 0;
                while (e1 != null && indexErrorMessage < maxErrorMessages)
                {
                    dailyTaskScheduleResponse.Errors.Add(e1.Message);
                    e1 = e1.InnerException;
                    indexErrorMessage++;
                }
                return dailyTaskScheduleResponse;
            }
        }

        public static void CopyFrom(this DailyTaskScheduleDetail scheduleDetail, DailyTaskScheduleDetail src)
        {
            scheduleDetail.Info = src.Info;
            scheduleDetail.OrderId = src.OrderId;
        }

        public static async Task<DailyTaskScheduleResponse> UpdateDailyTaskSchedules(this WordDbContext context, DailyTaskScheduleSubmit data, ILogger logger)
        {
            DailyTaskScheduleResponse dailyTaskScheduleResponse = new DailyTaskScheduleResponse();
            dailyTaskScheduleResponse.DailyTaskScheduleSubmit = data;
            try
            {
                foreach (var scheduleUnit in data.DailyTaskScheduleUnits)
                {
                    if (context.DailyTaskSchedules.Any(s =>
                        s.DailyTaskId == scheduleUnit.DailyTaskSchedule.DailyTaskId &&
                        s.ActDate.Year == scheduleUnit.DailyTaskSchedule.ActDate.Year &&
                        s.ActDate.DayOfYear == scheduleUnit.DailyTaskSchedule.ActDate.DayOfYear))
                    {
                        var taskSchedule = context.DailyTaskSchedules.First(s =>
                            s.DailyTaskId == scheduleUnit.DailyTaskSchedule.DailyTaskId &&
                            s.ActDate.Year == scheduleUnit.DailyTaskSchedule.ActDate.Year &&
                            s.ActDate.DayOfYear == scheduleUnit.DailyTaskSchedule.ActDate.DayOfYear);
                        taskSchedule.Info = scheduleUnit.DailyTaskSchedule.Info;
                        if (taskSchedule.Id != scheduleUnit.DailyTaskSchedule.Id)
                        {
                            scheduleUnit.DailyTaskSchedule.Id = taskSchedule.Id;
                        }
                    }
                    else
                    {
                        await context.DailyTaskSchedules.AddAsync(scheduleUnit.DailyTaskSchedule);
                    }
                    await context.SaveChangesAsync();

                    foreach (var detail in scheduleUnit.DailyTaskScheduleDetails)
                    {
                        detail.DailyTaskScheduleId = scheduleUnit.DailyTaskSchedule.Id;
                        if (context.DailyTaskScheduleDetails.Any(d => d.DailyTaskScheduleId == detail.DailyTaskScheduleId &&
                            d.DailyTaskSubId == detail.DailyTaskSubId))
                        {
                            // Edited schedule detail
                            var scheduleDetail = context.DailyTaskScheduleDetails.First(d => d.DailyTaskScheduleId == detail.DailyTaskScheduleId &&
                                d.DailyTaskSubId == detail.DailyTaskSubId);
                            scheduleDetail.CopyFrom(detail);
                            if (scheduleDetail.Id != detail.Id)
                            {
                                detail.Id = scheduleDetail.Id;
                            }
                        }
                        else
                        {
                            // New schedule detail
                            detail.Id = 0;
                            await context.DailyTaskScheduleDetails.AddAsync(detail);
                        }
                        await context.SaveChangesAsync();
                    }

                    // Deleted schedule detail
                    var newAllDetails = context.DailyTaskScheduleDetails.Where(d => d.DailyTaskScheduleId == scheduleUnit.DailyTaskSchedule.Id).ToList();
                    var toDelete = newAllDetails.Where(s => !scheduleUnit.DailyTaskScheduleDetails.Any(s2 => s2.DailyTaskScheduleId == s.DailyTaskScheduleId && s2.DailyTaskSubId == s.DailyTaskSubId && s2.Id == s.Id));
                    if (toDelete.Count() > 0)
                    {
                        context.DailyTaskScheduleDetails.RemoveRange(toDelete);
                        await context.SaveChangesAsync();
                    }

                }

                // Deleted whole schedule
                var newAllSchedules = context.DailyTaskSchedules.Where(s => s.DailyTaskId == data.TaskId).ToList();
                var toDeleteSchedules = newAllSchedules.Where(s => !data.DailyTaskScheduleUnits.Any(s2 => s2.DailyTaskSchedule.ActDate.Year == s.ActDate.Year && s2.DailyTaskSchedule.ActDate.DayOfYear == s.ActDate.DayOfYear));
                if (toDeleteSchedules.Count() > 0)
                {
                    foreach(var toDeleteSchedule in toDeleteSchedules)
                    {
                        context.DailyTaskScheduleDetails.RemoveRange(context.DailyTaskScheduleDetails.Where(d => d.DailyTaskScheduleId == toDeleteSchedule.Id));
                        await context.SaveChangesAsync();
                    }
                    context.DailyTaskSchedules.RemoveRange(toDeleteSchedules);
                    await context.SaveChangesAsync();
                }

                return dailyTaskScheduleResponse;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", data);
                dailyTaskScheduleResponse.Errors.Add(e.Message);
                Exception e1 = e.InnerException;
                int maxErrorMessages = 50;
                int indexErrorMessage = 0;
                while (e1 != null && indexErrorMessage < maxErrorMessages)
                {
                    dailyTaskScheduleResponse.Errors.Add(e1.Message);
                    e1 = e1.InnerException;
                    indexErrorMessage++;
                }
                return dailyTaskScheduleResponse;
            }
        }
    }
}

