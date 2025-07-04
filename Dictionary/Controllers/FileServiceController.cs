using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dictionary.Migrations;
using Dictionary.Models;
using Dictionary.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dictionary.Controllers
{
    [ApiController]
    [Route("pubfile")]
    public class FileServiceController : Controller
    {
        private readonly ILogger<FileServiceController> _logger;
        private readonly WordDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private int _uploadTimeOut = 30;

        public FileServiceController(WordDbContext context, ILogger<FileServiceController> logger, IConfiguration configuration, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _uploadTimeOut = _configuration.GetValue<int>("UploadSettings:TimeOut");
        }

        [HttpGet("getstatuses")]
        public ActionResult GetStatuses()
        {
            List<ServiceFileStatus> s = _context.ServiceFileStatuses.ToList();
            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(s);
            }
        }

        // prepare unique token to upload file
        [HttpPost("prepareupload")]
        public async Task<ActionResult> PrepareUpload(ServiceFile serviceFile)
        {
            DateTime dtNow = DateTime.Now;
            ServiceFile copiedSF = serviceFile.NewCopy();
            copiedSF.Id = 0;
            copiedSF.Status = 1;
            copiedSF.Time = dtNow;
            copiedSF.UId = NewUId(dtNow);

            var s = await _context.ServiceFiles.AddAsync(copiedSF);
            await _context.SaveChangesAsync();

            if (s == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(copiedSF);
            }
        }

        private string NewUId(DateTime dateTime)
        {
            return dateTime.Year.ToString() + dateTime.DayOfYear.ToString().PadLeft(3, '0') +
                "-" + Guid.NewGuid().ToString() +
                "-" + dateTime.ToString("HHmmss");
        }

        [HttpPut("reactivateupload/{id}/{uId}")]
        public async Task<ActionResult> ReactivateUpload(int id, string uId)
        {
            var s = _context.ServiceFiles.FirstOrDefault(s => s.Id == id && s.UId == uId && (s.Status == 1 || s.Status == 3));

            DateTime dtNow = DateTime.Now;
            if (s != null)
            {
                s.Status = 1;
                s.Time = dtNow;
                s.UId = NewUId(dtNow);
                await _context.SaveChangesAsync();

                return new OkObjectResult(s);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        // Upload file
        [HttpPost("uploadfile")]
        public async Task<ActionResult> Upload([FromForm] IFormFile file, [FromForm] int id, [FromForm] string uId)
        {
            if (file != null && file.Length > 0)
            {
                var s = _context.ServiceFiles.FirstOrDefault(s => s.Id == id && s.UId == uId);

                if (s != null)
                {
                    TimeSpan timeSpan = DateTime.Now - s.Time;
                    if (timeSpan.Seconds >= _uploadTimeOut)
                    {
                        return BadRequest(new { Message = "Time out, please retry.", Id = id, UId = uId });
                    }

                    decimal decReturnDId = 0;
                    // 处理文件上传逻辑，例如保存到服务器或数据库等
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        byte[] fileBytes = stream.ToArray();

                        string fileName = file.FileName;
                        string fileType = file.ContentType;
                        string query = "BEGIN INSERT INTO ServiceFileDatas (ServiceFileId, FileName, FileType, FileData, UploadTime) " +
                                "VALUES (@ServiceFileId, @FileName, @FileType, @FileData, @UploadTime); " +
                                "SELECT SCOPE_IDENTITY() AS DID; " +
                                "END; ";
                        using (SqlConnection con = new SqlConnection(_context.Database.GetConnectionString()))
                        {
                            SqlCommand cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@ServiceFileId", id);
                            cmd.Parameters.AddWithValue("@FileName", fileName);
                            cmd.Parameters.AddWithValue("@FileType", fileType);
                            cmd.Parameters.AddWithValue("@FileData", fileBytes);
                            cmd.Parameters.AddWithValue("@UploadTime", DateTime.Now);
                            con.Open();
                            try
                            {
                                //await cmd.ExecuteNonQueryAsync();
                                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                                {
                                    if (reader.Read())
                                    {
                                        decReturnDId = reader.GetDecimal(0);
                                    }
                                }
                                
                            }
                            catch(Exception e)
                            {
                                return Conflict(new { Message = e.Message, Id = id, UId = uId, DId = decReturnDId });
                            }
                        }
                    }
                    s.Status = 2;
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "File uploaded successfully", Id = id, UId = uId, DId = (int)decReturnDId });
                }
                else
                {
                    return NotFound(new { Message = "Failed to upload file", Id = id, UId = uId, DId = 0 });
                }
            }
            else
            {
                return BadRequest(new { Message = "Failed to upload an empty or null file", Id = id, UId = uId, DId = 0 });
            }
        }

        [HttpGet("getfiles")]
        public async Task<ActionResult> GetFiles(QueryConds queryConds)
        {
            IQueryable<ServiceFileOut> s = null;
            if (queryConds.QueryType == (int)QueryType.ByDate)
            {
                int dateType = Int32.Parse(queryConds.QueryParams[1]);
                DateTime start = DateTime.Parse(queryConds.QueryParams[2]);
                DateTime end = DateTime.Parse(queryConds.QueryParams[3]).AddDays(1);
                if (dateType == (int)QueryDateType.ByStartDate) // UploadTime
                {
                    s = from t1 in _context.ServiceFiles.Where(sf => sf.Status == 2)
                             from t2 in _context.ServiceFileDatas.Where(sfd => sfd.ServiceFileId == t1.Id && sfd.UploadTime < end && sfd.UploadTime >= start)
                             select new ServiceFileOut {
                                 Id = t1.Id,
                                 UId = t1.UId,
                                 ServiceName = t1.ServiceName,
                                 Category = t1.Category,
                                 ServiceKey = t1.ServiceKey,
                                 ServiceValue = t1.ServiceValue,
                                 FileName = t2.FileName,
                                 FileType = t2.FileType,
                                 Status = t1.Status,
                                 UploadTime = t2.UploadTime,
                                 Time = t1.Time,
                                 DId = t2.Id
                            };
                    //s = _context.ServiceFiles.Where(d => d.Status == 2 && d.UploadTime < end && d.UploadTime >= start);
                }
                else if (dateType == (int)QueryDateType.ByCreateTime) // Time
                {
                    s = from t1 in _context.ServiceFiles.Where(d => d.Status == 1 && d.Time < end && d.Time >= start)
                        select new ServiceFileOut
                        {
                            Id = t1.Id,
                            UId = t1.UId,
                            ServiceName = t1.ServiceName,
                            Category = t1.Category,
                            ServiceKey = t1.ServiceKey,
                            ServiceValue = t1.ServiceValue,
                            Status = t1.Status,
                            Time = t1.Time
                        };
                }
                else if (dateType == (int)QueryDateType.ByDoneTime) // DeleteTime
                {
                    s = from t1 in _context.ServiceFiles.Where(sf => sf.Status == 3)
                        from t2 in _context.ServiceFileDatas.Where(sfd => sfd.ServiceFileId == t1.Id && sfd.UploadTime < end && sfd.UploadTime >= start)
                        select new ServiceFileOut
                        {
                            Id = t1.Id,
                            UId = t1.UId,
                            ServiceName = t1.ServiceName,
                            Category = t1.Category,
                            ServiceKey = t1.ServiceKey,
                            ServiceValue = t1.ServiceValue,
                            FileName = t2.FileName,
                            FileType = t2.FileType,
                            Status = t1.Status,
                            UploadTime = t2.UploadTime,
                            Time = t1.Time,
                            DId = t2.Id
                        };
                    //s = _context.ServiceFiles.Where(d => d.Status == 3 && d.UploadTime < end && d.UploadTime >= start);
                }
            }
            else if (queryConds.QueryType == (int)QueryType.ById)
            {
                int Id = Int32.Parse(queryConds.QueryParams[1]);
                s = from t1 in _context.ServiceFiles.Where(sf => sf.Id == Id)
                    from t2 in _context.ServiceFileDatas.Where(sfd => sfd.ServiceFileId == Id)
                    select new ServiceFileOut
                    {
                        Id = t1.Id,
                        UId = t1.UId,
                        ServiceName = t1.ServiceName,
                        Category = t1.Category,
                        ServiceKey = t1.ServiceKey,
                        ServiceValue = t1.ServiceValue,
                        FileName = t2.FileName,
                        FileType = t2.FileType,
                        Status = t1.Status,
                        UploadTime = t2.UploadTime,
                        Time = t1.Time,
                        DId = t2.Id
                    };
                //s = _context.ServiceFiles.Where(d => d.Id == Id);
            }
            else if (queryConds.QueryType == (int)QueryType.ByName)
            {
                string fileName = queryConds.QueryParams[1];
                s = from t1 in _context.ServiceFiles.Where(sf => sf.Status == 2 || sf.Status == 3)
                    from t2 in _context.ServiceFileDatas.Where(sfd => sfd.ServiceFileId == t1.Id && sfd.FileName.IndexOf(fileName) > -1)
                    select new ServiceFileOut
                    {
                        Id = t1.Id,
                        UId = t1.UId,
                        ServiceName = t1.ServiceName,
                        Category = t1.Category,
                        ServiceKey = t1.ServiceKey,
                        ServiceValue = t1.ServiceValue,
                        FileName = t2.FileName,
                        FileType = t2.FileType,
                        Status = t1.Status,
                        UploadTime = t2.UploadTime,
                        Time = t1.Time,
                        DId = t2.Id
                    };
                //s = _context.ServiceFiles.Where(d => (d.Status == 2 || d.Status == 3) && d.FileName.IndexOf(fileName) > -1);
            }
            else if (queryConds.QueryType == (int)QueryType.ByCombination)
            {
                int year = Int32.Parse(queryConds.QueryParams[1]);
                DateTime endYearStart = new DateTime(year, 1, 1, 0, 0, 0);
                string fileName = queryConds.QueryParams[2];
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    s = from t1 in _context.ServiceFiles.Where(sf => sf.Status == 2 || sf.Status == 3)
                        from t2 in _context.ServiceFileDatas.Where(sfd => sfd.ServiceFileId == t1.Id && sfd.UploadTime.Year <= year
                            && sfd.UploadTime >= endYearStart && sfd.FileName.IndexOf(fileName) > -1)
                        select new ServiceFileOut
                        {
                            Id = t1.Id,
                            UId = t1.UId,
                            ServiceName = t1.ServiceName,
                            Category = t1.Category,
                            ServiceKey = t1.ServiceKey,
                            ServiceValue = t1.ServiceValue,
                            FileName = t2.FileName,
                            FileType = t2.FileType,
                            Status = t1.Status,
                            UploadTime = t2.UploadTime,
                            Time = t1.Time,
                            DId = t2.Id
                        };
                }
                else
                {
                    s = from t1 in _context.ServiceFiles.Where(sf => sf.Status == 2 || sf.Status == 3)
                        from t2 in _context.ServiceFileDatas.Where(sfd => sfd.ServiceFileId == t1.Id && sfd.UploadTime.Year <= year
                            && sfd.UploadTime >= endYearStart)
                        select new ServiceFileOut
                        {
                            Id = t1.Id,
                            UId = t1.UId,
                            ServiceName = t1.ServiceName,
                            Category = t1.Category,
                            ServiceKey = t1.ServiceKey,
                            ServiceValue = t1.ServiceValue,
                            FileName = t2.FileName,
                            FileType = t2.FileType,
                            Status = t1.Status,
                            UploadTime = t2.UploadTime,
                            Time = t1.Time,
                            DId = t2.Id
                        };
                }
            }

            return await GetQueryResult(queryConds, s);
        }

        private async Task<ActionResult> GetQueryResult(QueryConds queryConds, IQueryable<ServiceFileOut> s)
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

                    int countPerPage = await _context.GetParameter<int>("ServiceFile",
                        "CountPerPage", 20);

                    PageInfo pageInfo = new()
                    {
                        CountPerPage = countPerPage,
                        CurrentPage = queryConds.QueryPage,
                        TotalCount = totalCount,
                        TotalPageCount = totalCount / countPerPage + (totalCount % countPerPage == 0 ? 0 : 1)
                    };

                    int minimumCountToSplitIntoPages = await _context.GetParameter<int>("ServiceFile",
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
                    List<ServiceFileOut> ls = se.ToList();
                    PageData<ServiceFileOut> pageData = new()
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

        [HttpGet("getstream/{id}/{uId}/{fId}")]
        public IActionResult GetFileStream(int id, string uId, int fId)
        {
            var file = _context.ServiceFiles.FirstOrDefault(sf => sf.Id == id && sf.UId == uId);
            if (file == null) return NotFound();

            var fileData = _context.ServiceFileDatas.FirstOrDefault(fd => fd.Id == fId && fd.ServiceFileId == id);
            if (fileData == null) return NotFound();

            string tempFilePath = Path.Combine(Path.GetTempPath(), file.UId); // 创建临时文件路径
            System.IO.File.WriteAllBytes(tempFilePath, fileData.FileData); // 将字节写入临时文件

            try
            {
                string contentType = fileData.FileType; // "application/octet-stream"; // 根据需要设置正确的MIME类型
                var result = new FileStreamResult(new FileStream(tempFilePath, FileMode.Open, FileAccess.Read), contentType)
                {
                    FileDownloadName = fileData.FileName // 设置下载文件名
                };
                return result;
            }
            finally
            {
                System.IO.File.Delete(tempFilePath); // 清理临时文件
            }

            //return File(file.FileData, file.FileType, file.FileName); // 设置正确的MIME类型和文件名
        }

        [HttpGet("get/{id}/{uId}/{fId}")]
        public IActionResult GetFile(int id, string uId, int fId)
        {
            var file = _context.ServiceFiles.FirstOrDefault(sf => sf.Id == id && sf.UId == uId);
            if (file == null) return NotFound();

            var fileData = _context.ServiceFileDatas.FirstOrDefault(fd => fd.Id == fId && fd.ServiceFileId == id);
            if (fileData == null) return NotFound();

            return File(fileData.FileData, fileData.FileType, fileData.FileName);
        }

        [HttpPut("update")]
        public async Task<ActionResult<ServiceFile>> UpdateServiceFile(ServiceFile serviceFile)
        {
            var file = _context.ServiceFiles.FirstOrDefault(f => f.Id == serviceFile.Id && f.UId == serviceFile.UId);
            if (file == null) return NotFound();

            file.CopyFrom(serviceFile);

            await _context.SaveChangesAsync();
            await _context.Entry(file).ReloadAsync();

            return file;
        }

        [HttpDelete("delete/{id}/{uId}")]
        public IActionResult DeleteFile(int id, string uId, int setFlagOnly = 1)
        {
            var file = _context.ServiceFiles.FirstOrDefault(f => f.Id == id && f.UId == uId);
            if (file == null) return NotFound();

            if (setFlagOnly == 1)
            {
                file.Status = 3;
                _context.SaveChanges();
            }
            else
            {
                var fileDatas = _context.ServiceFileDatas.Where(fd => fd.ServiceFileId == id);
                foreach(var fd in fileDatas)
                {
                    _context.ServiceFileDatas.Remove(fd);
                }
                _context.ServiceFiles.Remove(file);
                _context.SaveChanges();
            }
            return Ok(); 
        }

        [HttpPut("removeobsoleted/{id}/{uId}")]
        public async Task<IActionResult> RemoveObsoletedFiles(int id, string uId, List<int> validDIds)
        {
            var file = _context.ServiceFiles.FirstOrDefault(f => f.Id == id && f.UId == uId);
            if (file == null) return NotFound();

            string dIds = string.Join(',', validDIds);
            string query = "DELETE FROM ServiceFileDatas WHERE ServiceFileId=@ServiceFileId AND Id NOT IN ("+dIds+")";
            using (SqlConnection con = new SqlConnection(_context.Database.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ServiceFileId", id);
                con.Open();
                await cmd.ExecuteNonQueryAsync();
            }

            return Ok();
        }
    }
}

