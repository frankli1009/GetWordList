using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace Dictionary.Models
{
    public class DailyTaskSubExtraInfo
    {
        public DailyTaskSubExtraInfo()
        {
        }
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(254)")]
        public string Info { get; set; }
        public int OrderId { get; set; }
        public int DailyTaskId { get; set; }

        public int DailyTaskSubId { get; set; }
        public virtual DailyTaskSub DailyTaskSub { get; set; }
    }
}


