using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Microsoft.Extensions.Logging;

namespace Dictionary.Utilities
{
    public static class SudokuExtensions
    {
        public static int GetSudokuCount(this WordDbContext context)
        {
            return (from s in context.Sudokus select s.Id).Count();
        }

        public static Sudoku GetSudoku(this WordDbContext context, int id)
        {
            return (from s in context.Sudokus where s.Id == id select s).FirstOrDefault();
        }

        public static async Task<Sudoku> AddSudoku(this WordDbContext context, string data, ILogger logger)
        {
            Sudoku sudoku = new Sudoku() { Data = data, SudokuTypeId = 1 };
            try
            {
                if (context.Sudokus.Any(s => s.Data == data))
                {
                    sudoku = context.Sudokus.First(s => s.Data == data);
                    logger.LogWarning("Skip adding data which already exists/{0}|{1}", sudoku.Id, data);
                    return sudoku;
                }
                else if (context.SudokuRecyclable.Any())
                {
                    int id = context.SudokuRecyclable.First().SudokuId;
                    sudoku = await UpdateSudoku(context, id, data, logger);
                    context.SudokuRecyclable.Remove(context.SudokuRecyclable.First());
                    await context.SaveChangesAsync();
                    logger.LogWarning("Update data to recyclable id/{0}|{1}", id, data);
                    return sudoku;
                }
                await context.Sudokus.AddAsync(sudoku);
                await context.SaveChangesAsync();
                return sudoku;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", data);
                return null;
            }
        }

        public static async Task<Sudoku> UpdateSudoku(this WordDbContext context, int id, string data, ILogger logger)
        {
            Sudoku sudoku = context.Sudokus.First(s => s.Id == id);
            try
            {
                sudoku.Data = data;
                await context.SaveChangesAsync();
                return sudoku;
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to add/{0}", data);
                return null;
            }
        }

        public static async Task<int> AddSudokuBatch(this WordDbContext context, IEnumerable<string> datas, ILogger logger)
        {
            int success = 0;
            foreach (var data in datas)
            {
                var sudoku = await AddSudoku(context, data, logger);
                if (sudoku != null)
                {
                    success++;
                }
            }
            return success;
        }

        public static async Task<bool> DeleteSudoku(this WordDbContext context, string data)
        {
            if (context.Sudokus.Any(s => s.Data == data))
            {
                List<Sudoku> sudokus = context.Sudokus.Where(s => s.Data == data).ToList();
                context.Sudokus.RemoveRange(sudokus);
                await context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
