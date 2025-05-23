using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dictionary.Utilities
{
	public static class DeliveryExtensions
	{
        public static async Task<Delivery> AddDelivery(this WordDbContext context, Delivery data, ILogger logger)
        {
            try
            {
                await context.Deliveries.AddAsync(data);
                await context.SaveChangesAsync();

                await context.Entry(data).ReloadAsync();
                return data;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add delivery data/{0}", data);
                return null;
            }
        }
    }
}

