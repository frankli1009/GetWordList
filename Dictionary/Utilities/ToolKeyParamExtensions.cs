using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dictionary.Utilities
{
	public static class ToolKeyParamExtensions
    {
        public static void CopyFrom(this ToolKeyParam param, ToolKeyParam src)
        {
            param.Category = src.Category;
            param.Key = src.Key;
            param.Parameters = src.Parameters;
            param.Info = src.Info;
        }

		public static async Task<ToolKeyParam> GetToolKeyParam(this WordDbContext context, string category, string key)
		{
            if (await context.ToolKeyParams.AnyAsync(p => p.Category.ToLower() == category.ToLower() && p.Key.ToLower() == key.ToLower()))
            {
                ToolKeyParam tkp = await context.ToolKeyParams.FirstOrDefaultAsync(p => p.Category.ToLower() == category.ToLower() && p.Key.ToLower() == key.ToLower());
                return tkp;
            }
            else
            {
                return null;
            }
        }

        public static async Task<OperationResult<ToolKeyParam>> AddToolKeyParam(this WordDbContext context, ToolKeyParam param, ILogger logger)
        {
            OperationResult<ToolKeyParam> or = new OperationResult<ToolKeyParam>();
            try
            {
                await context.ToolKeyParams.AddAsync(param);
                await context.SaveChangesAsync();
                or.Oks.Add(param);
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}={1}", param.Key, param.Parameters);
                or.UnknownErrors.Add(param);
            }
            return or;
        }

        public static async Task<OperationResult<ToolKeyParam>> UpdateToolKeyParam(this WordDbContext context, ToolKeyParam param, ILogger logger)
        {
            OperationResult<ToolKeyParam> or = new OperationResult<ToolKeyParam>();
            try
            {
                if (await context.ToolKeyParams.AnyAsync(p => p.Id == param.Id))
                {
                    ToolKeyParam param1 = await context.ToolKeyParams.FirstOrDefaultAsync(p => p.Id == param.Id);
                    param1.CopyFrom(param);
                    await context.SaveChangesAsync();
                    or.Oks.Add(param);
                }
                else
                {
                    logger.LogError(new Exception("Not found"), "failed to update/{0}.{1}={2}", param.Category, param.Key, param.Parameters);
                    or.NotFounds.Add($"Tool key parameters not found. id={param.Id}, key={param.Key}");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to update/{0}.{1}={2}", param.Category, param.Key, param.Parameters);
                or.UnknownErrors.Add(param);
            }
            return or;
        }

        public static async Task<OperationResult<ToolKeyParam>> DeleteToolKeyParam(this WordDbContext context, ToolKeyParam param, ILogger logger)
        {
            OperationResult<ToolKeyParam> or = new OperationResult<ToolKeyParam>();
            try
            {
                if (await context.ToolKeyParams.AnyAsync(p => p.Id == param.Id))
                {
                    ToolKeyParam param1 = await context.ToolKeyParams.FirstOrDefaultAsync(p => p.Id == param.Id);
                    context.ToolKeyParams.Remove(param1);
                    await context.SaveChangesAsync();
                    or.Oks.Add(param);
                }
                else
                {
                    logger.LogError(new Exception("Not found"), "failed to delete/{0}.{1}={2}", param.Category, param.Key, param.Parameters);
                    or.NotFounds.Add($"Tool key parameters not found. id={param.Id}, key={param.Key}");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to delete/{0}.{1}={2}", param.Category, param.Key, param.Parameters);
                or.UnknownErrors.Add(param);
            }
            return or;
        }

        public static async Task<T> GetParameter<T>(this WordDbContext context, string category, string key, T defaultValue)
        {

            ToolKeyParam tkpCountPerPage = await context.GetToolKeyParam(category, key);
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

    }
}

