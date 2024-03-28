using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return context.DailyTaskSubs.Where(s => s.DailyTaskId == taskId).OrderBy(s => s.Id).ToList();
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
    }
}

