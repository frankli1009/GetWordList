﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dictionary.Utilities
{
	public static class EnergyDataExtensions
	{
        public static int GetEnergyDataCount(this WordDbContext context)
        {
            return (from s in context.ConsumerGoodsDetails select s.Id).Count();
        }

        public static ConsumerGoodsDetail GetEnergyData(this WordDbContext context, int Id)
        {
            return context.ConsumerGoodsDetails.Where<ConsumerGoodsDetail>(s => s.Id == Id).First();
        }

        public static List<ConsumerGoodsDetail> GetEnergyData(this WordDbContext context, int energyTypeId, int year, int month)
        {
            int ENERGY_DATA_STARTDATE = 19;
            int day = ENERGY_DATA_STARTDATE;
            int startYear, startMonth;
            int endYear, endMonth;
            DateTime startTime, endTime;
            if (day == 1)
            {
                startTime = new DateTime(year, month, day);
                endMonth = month + 1;
                if (endMonth > 12)
                {
                    endYear = year + 1;
                    endMonth = 1;
                }
                else
                {
                    endYear = year;
                }
                endTime = new DateTime(endYear, endMonth, day);
            }
            else
            {
                endTime = new DateTime(year, month, day);
                startMonth = month - 1;
                if (startMonth < 1)
                {
                    startYear = year - 1;
                    startMonth = 12;
                }
                else
                {
                    startYear = year;
                }
                startTime = new DateTime(startYear, startMonth, day);
            }
            return context.ConsumerGoodsDetails.Where<ConsumerGoodsDetail>(s =>s.ConsumerGoodsId == energyTypeId && s.Time > startTime && s.Time < endTime).ToList<ConsumerGoodsDetail>();
        }

        public static async Task<ConsumerGoodsDetail> AddEnergyData(this WordDbContext context, ConsumerGoodsDetail data, ILogger logger)
        {
            try
            {
                await context.ConsumerGoodsDetails.AddAsync(data);
                await context.SaveChangesAsync();

                await context.Entry(data).ReloadAsync();
                return data;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", data);
                return null;
            }
        }

    }
}

