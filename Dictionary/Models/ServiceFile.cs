using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
    public class ServiceFile
    {
        public ServiceFile()
        {
        }
        public int Id { get; set; }
        [Column(TypeName = "varchar(64)")]
        public string UId { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string ServiceName { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string Category { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string ServiceKey { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string ServiceValue { get; set; }
        public DateTime Time { get; set; }
        public int Status { get; set; }
    }
}

