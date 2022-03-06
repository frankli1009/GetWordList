using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Models
{
    public class SudokuRecyclable
    {
        public SudokuRecyclable()
        {
        }

        public int Id { get; set; }
        public int SudokuId { get; set; }
        public virtual Sudoku Sudoku { get; set; }
    }
}
