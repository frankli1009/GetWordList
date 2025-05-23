using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
    public class Delivery
    {
        public Delivery()
        {
        }
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(254)")]
        public string ProductName { get; set; }
        [Column(TypeName = "varchar(254)")]
        public string DeliveryNo { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public DateTime? CancelTime { get; set; }
        public int StatusId { get; set; }
        [Column(TypeName = "nvarchar(254)")]
        public string Info { get; set; }
        public DateTime Time { get; set; }
    }
}

