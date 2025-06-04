using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Dictionary.Models;
using Dictionary.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dictionary.Controllers
{
    [ApiController]
    [Route("delivery")]
    public class DeliveryController : Controller
    {
        private readonly ILogger<DeliveryController> _logger;
        private readonly WordDbContext _context;

        public DeliveryController(WordDbContext context, ILogger<DeliveryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("getstatuses")]
        public ActionResult GetDeliveryStatuses()
        {
            List<DeliveryStatus> s = _context.DeliveryStatuses.ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpGet("get/{id}")]
        public ActionResult GetDelivery(int id)
        {
            Delivery s = _context.Deliveries.Where(t => t.Id == id).FirstOrDefault();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        [HttpGet("getdeliverieson")]
        public ActionResult GetDeliveriesOn(int orderType=0, string orderField="")
        {
            List<Delivery> ls;
            var s = _context.Deliveries.Where(d => d.StatusId < 4).AsEnumerable();
            if (orderType == (int)ResultOrderType.None || string.IsNullOrWhiteSpace(orderField))
            {
                ls = s.OrderByDescending(d => d.Id).ToList();
            }
            else if (orderType == (int)ResultOrderType.Ascending)
            {
                ls = s.OrderBy(d => d.GetPropertyValue(orderField)).ToList();
            }
            else
            {
                ls = s.OrderByDescending(d => d.GetPropertyValue(orderField)).ToList();
            }
                
            if (ls == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(ls);
            }
        }

        [HttpGet("getdeliverieshist")]
        public async Task<ActionResult> GetDeliveriesHist(QueryConds queryConds)
        {
            IQueryable<Delivery> s = null;
            if (queryConds.QueryType == (int)QueryType.ByDate)
            {
                int dateType = Int32.Parse(queryConds.QueryParams[1]);
                DateTime start = DateTime.Parse(queryConds.QueryParams[2]);
                DateTime end = DateTime.Parse(queryConds.QueryParams[3]).AddDays(1);
                if (dateType == (int)QueryDateType.ByStartDate) // OrderTime
                {
                    s = _context.Deliveries.Where(d => (d.StatusId == 4 || d.StatusId == 5) && d.OrderTime < end && d.OrderTime >= start);
                }
                else if (dateType == (int)QueryDateType.ByEndDate) // ReceiveDate
                {
                    s = _context.Deliveries.Where(d => (d.StatusId == 4 || d.StatusId == 5) && d.ReceiveTime < end && d.ReceiveTime >= start);
                }
                else if (dateType == (int)QueryDateType.ByCreateTime) // Time
                {
                    s = _context.Deliveries.Where(d => (d.StatusId == 4 || d.StatusId == 5) && d.Time < end && d.Time >= start);
                }
                else if (dateType == (int)QueryDateType.ByDoneTime) // CancelTime
                {
                    s = _context.Deliveries.Where(d => (d.StatusId == 4 || d.StatusId == 5) && d.CancelTime < end && d.CancelTime >= start);
                }
                else // StartDate >= EndDate <=
                {
                    s = _context.Deliveries.Where(d => (d.StatusId == 4 || d.StatusId == 5) && d.ReceiveTime < end && d.OrderTime >= start);
                }
            }
            else if (queryConds.QueryType == (int)QueryType.ById)
            {
                int Id = Int32.Parse(queryConds.QueryParams[1]);
                s = _context.Deliveries.Where(d => (d.StatusId == 4 || d.StatusId == 5) && d.Id == Id);
            }
            else if (queryConds.QueryType == (int)QueryType.ByName)
            {
                string productName = queryConds.QueryParams[1];
                s = _context.Deliveries.Where(d => (d.StatusId == 4 || d.StatusId == 5) && d.ProductName.IndexOf(productName) > -1);
            }
            else if (queryConds.QueryType == (int)QueryType.ByCombination)
            {
                int year = Int32.Parse(queryConds.QueryParams[1]);
                DateTime endYearStart = new DateTime(year, 1, 1, 0, 0, 0);
                string productName = queryConds.QueryParams[2];
                s = _context.Deliveries.Where(d => d.OrderTime.Year <= year && ((d.StatusId == 4 && d.ReceiveTime >= endYearStart) || (d.StatusId == 5 && d.CancelTime >= endYearStart)));
                if (!string.IsNullOrWhiteSpace(productName)) s = s.Where(d => d.ProductName.IndexOf(productName) > -1);
            }

            return await GetQueryResult(queryConds, s);
        }

        private async Task<ActionResult> GetQueryResult(QueryConds queryConds, IQueryable<Delivery> s)
        {
            int queryTheme = Int32.Parse(queryConds.QueryParams[0]);
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                int totalCount = s.Count();
                if (totalCount > 0)
                {
                    var se = s.AsEnumerable();
                    if (!string.IsNullOrWhiteSpace(queryConds.OrderField))
                    {
                        if (queryConds.OrderType == (int)ResultOrderType.Ascending)
                        {
                            se = se.OrderBy(t => t.GetPropertyValue(queryConds.OrderField));
                        }
                        else if (queryConds.OrderType == (int)ResultOrderType.Descending)
                        {
                            se = se.OrderByDescending(t => t.GetPropertyValue(queryConds.OrderField));
                        }
                    }
                    else
                    {
                        se = se.OrderByDescending(t => t.Id);
                    }

                    int countPerPage = await _context.GetParameter<int>("Delivery",
                        "CountPerPage", 20);

                    PageInfo pageInfo = new()
                    {
                        CountPerPage = countPerPage,
                        CurrentPage = queryConds.QueryPage,
                        TotalCount = totalCount,
                        TotalPageCount = totalCount / countPerPage + (totalCount % countPerPage == 0 ? 0 : 1)
                    };

                    int minimumCountToSplitIntoPages = await _context.GetParameter<int>("Delivery",
                        "MinimumCountToSplitIntoPages", 30);
                    if (totalCount > minimumCountToSplitIntoPages)
                    {
                        if (totalCount > countPerPage * (queryConds.QueryPage - 1))
                        {
                            if (totalCount <= countPerPage * queryConds.QueryPage)
                            {
                                se = se.Skip(countPerPage * (queryConds.QueryPage - 1));
                            }
                            else
                            {
                                se = se.Skip(countPerPage * (queryConds.QueryPage - 1)).Take(countPerPage);
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
                    List<Delivery> ls = se.ToList();
                    PageData<Delivery> pageData = new()
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

        [HttpGet("getdeliveriesall")]
        public async Task<ActionResult> GetDeliveriesAll(QueryConds queryConds)
        {
            IQueryable<Delivery> s = null;
            if (queryConds.QueryType == (int)QueryType.ByDate)
            {
                int dateType = Int32.Parse(queryConds.QueryParams[1]);
                DateTime start = DateTime.Parse(queryConds.QueryParams[2]);
                DateTime end = DateTime.Parse(queryConds.QueryParams[3]).AddDays(1);
                if (dateType == (int)QueryDateType.ByStartDate) // OrderTime
                {
                    s = _context.Deliveries.Where(d => d.OrderTime < end && d.OrderTime >= start);
                }
                else if (dateType == (int)QueryDateType.ByEndDate) // ReceiveDate
                {
                    s = _context.Deliveries.Where(d => d.StatusId == 4 && d.ReceiveTime < end && d.ReceiveTime >= start);
                }
                else if (dateType == (int)QueryDateType.ByCreateTime) // Time
                {
                    s = _context.Deliveries.Where(d => d.Time < end && d.Time >= start);
                }
                else if (dateType == (int)QueryDateType.ByDoneTime) // CancelTime
                {
                    s = _context.Deliveries.Where(d => d.StatusId == 5 && d.CancelTime < end && d.CancelTime >= start);
                }
                else // StartDate >= EndDate <=
                {
                    s = _context.Deliveries.Where(d => d.StatusId == 4 && d.ReceiveTime < end && d.OrderTime >= start);
                }
            }
            else if (queryConds.QueryType == (int)QueryType.ById)
            {
                int Id = Int32.Parse(queryConds.QueryParams[1]);
                s = _context.Deliveries.Where(d => d.Id == Id);
            }
            else if (queryConds.QueryType == (int)QueryType.ByName)
            {
                string productName = queryConds.QueryParams[1];
                s = _context.Deliveries.Where(d => d.ProductName.IndexOf(productName) > -1);
            }
            else if (queryConds.QueryType == (int)QueryType.ByCombination)
            {
                int year = Int32.Parse(queryConds.QueryParams[1]);
                DateTime endYearStart = new DateTime(year, 1, 1, 0, 0, 0);
                string productName = queryConds.QueryParams[2];
                s = _context.Deliveries.Where(d => d.OrderTime.Year <= year && ((d.StatusId == 4 && d.ReceiveTime >= endYearStart) || (d.StatusId == 5 && d.CancelTime >= endYearStart)));
                if (!string.IsNullOrWhiteSpace(productName)) s = s.Where(d => d.ProductName.IndexOf(productName) > -1);
            }

            return await GetQueryResult(queryConds, s);
        }

        [HttpPost("add")]
        public async Task<ActionResult<Delivery>> AddDelivery(Delivery delivery)
        {
            Delivery or = await _context.AddDelivery(delivery, _logger);
            if (!(or is null))
            {
                return new OkObjectResult(or);
            }
            else
            {
                return new BadRequestObjectResult(or);
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult<Delivery>> UpdateDelivery(Delivery delivery)
        {
            try
            {
                var delivery1 = await _context.Deliveries.FirstOrDefaultAsync(d => d.Id == delivery.Id);
                if (delivery1 != null)
                {
                    delivery1.CopyFrom(delivery);
                    _context.SaveChanges();
                    await _context.Entry(delivery1).ReloadAsync();

                    return new OkObjectResult(delivery1);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult($"Exception raised on updating delivery data with id={delivery.Id}. Exception message is {e.Message}.");
            }
        }
    }
}

