using System;
namespace Dictionary.Models
{
    public class ServiceFileWithoutData
    {
        public ServiceFileWithoutData()
        {
        }
        public int Id { get; set; }
        public string UId { get; set; }
        public string ServiceName { get; set; }
        public string Category { get; set; }
        public string ServiceKey { get; set; }
        public string ServiceValue { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime Time { get; set; }
        public int Status { get; set; }
        public DateTime UploadTime { get; set; }
    }
}

