using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
    public class ServiceFileData
	{
		public ServiceFileData()
		{
		}
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(254)")]
        public string FileName { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string FileType { get; set; }
        public DateTime UploadTime { get; set; }
        public byte[] FileData { get; set; }

        public int ServiceFileId { get; set; }
        public virtual ServiceFile ServiceFile { get; set; }
    }
}

