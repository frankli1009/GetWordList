using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Models
{
    public class SudokuType
    {
        public SudokuType()
        {
        }

        public int Id { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string TypeName { get; set; }
    }
}
