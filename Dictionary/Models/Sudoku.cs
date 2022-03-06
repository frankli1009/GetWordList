using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictionary.Models
{
    public class Sudoku
    {
        public Sudoku()
        {
        }

        public int Id { get; set; }
        [Column(TypeName = "varchar(82)")]
        public string Data { get; set; }

        public DateTime CreateTime { get; set; }

        public int SudokuTypeId { get; set; }
        public virtual SudokuType SudokuType { get; set; }
    }
}
