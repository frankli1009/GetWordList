using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Models
{
    public sealed class WordDbContext : DbContext
    {
        public WordDbContext(DbContextOptions<WordDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Word>()
                .HasIndex(c => c.WordW)
                .IsUnique();
        }

        public DbSet<Word> Words { get; set; }
        public DbSet<Sudoku> Sudokus { get; set; }
        public DbSet<SudokuType> SudokuType { get; set; }
        public DbSet<SudokuRecyclable> SudokuRecyclable { get; set; }
        public DbSet<ConsumerGoodsDetail> ConsumerGoodsDetails { get; set; }
        public DbSet<ConsumerGoods> ConsumerGoods { get; set; }
        public DbSet<ConsumerGoodsExtra> ConsumerGoodsExtra { get; set; }
        public DbSet<ConsumerGoodsParameters> ConsumerGoodsParameters { get; set; }
    }

}
