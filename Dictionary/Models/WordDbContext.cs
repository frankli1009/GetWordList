using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    }

    [Index(nameof(Length))]
    [Index(nameof(StartLetter))]
    public class Word
    {
        public Word() { }

        public Word(string word)
        {
            WordW = word;
            Length = (short)word.Length;
            StartLetter = word[0];
            EndLetter = word[word.Length - 1];
        }

        public int Id { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string WordW { get; set; }
        public char StartLetter { get; set; }
        public char EndLetter { get; set; }
        public short Length { get; set; }
    }
}
