using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Migrations;
using Dictionary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dictionary.Utilities
{
	public static class FileServiceExtensions
	{
		public static ServiceFile NewCopy(this ServiceFile serviceFile)
		{
			ServiceFile copiedSF = new ServiceFile()
			{
				Id = serviceFile.Id,
				UId = serviceFile.UId,
				ServiceName = serviceFile.ServiceName,
				Category = serviceFile.Category,
				ServiceKey = serviceFile.ServiceKey,
				ServiceValue = serviceFile.ServiceValue,
				Time = serviceFile.Time,
				Status = serviceFile.Status
			};

			return copiedSF;
		}

        public static ServiceFileData NewCopy(this ServiceFileData serviceFileData, bool withFileData = false)
        {
            ServiceFileData copiedSFD = new ServiceFileData()
            {
                Id = serviceFileData.Id,
                ServiceFileId = serviceFileData.ServiceFileId,
                UploadTime = serviceFileData.UploadTime
            };
            if (withFileData && serviceFileData.FileData != null && serviceFileData.FileData.Length > 0)
            {
                copiedSFD.FileName = serviceFileData.FileName;
                copiedSFD.FileType = serviceFileData.FileType;
                copiedSFD.FileData = new byte[serviceFileData.FileData.Length];
                Array.Copy(serviceFileData.FileData, copiedSFD.FileData, serviceFileData.FileData.Length);
            }

            return copiedSFD;
        }

        public static ServiceFile CopyFrom(this ServiceFile serviceFile, ServiceFile src)
        {
            serviceFile.Id = src.Id;
            serviceFile.ServiceName = src.ServiceName;
            serviceFile.ServiceKey = src.ServiceKey;
            serviceFile.ServiceValue = src.ServiceValue;
            serviceFile.UId = src.UId;
            serviceFile.Time = src.Time;
            serviceFile.Status = src.Status;

            return serviceFile;
        }

        public static ServiceFileData CopyFrom(this ServiceFileData serviceFileData, ServiceFileData src)
        {
            serviceFileData.Id = src.Id;
            serviceFileData.ServiceFileId = src.ServiceFileId;
            serviceFileData.UploadTime = src.UploadTime;
            serviceFileData.FileName = src.FileName;
            serviceFileData.FileType = src.FileType;

            serviceFileData.FileData = new byte[src.FileData.Length];
            if (src.FileData.Length > 0)
            {
                src.FileData.CopyTo(serviceFileData.FileData, 0);
            }

            return serviceFileData;
        }
    }
}

