﻿// <auto-generated />
using System;
using Dictionary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dictionary.Migrations
{
    [DbContext(typeof(WordDbContext))]
    [Migration("20220216060939_trSudokusSetTypeId")]
    partial class trSudokusSetTypeId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Dictionary.Models.Sudoku", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Data")
                        .HasColumnType("varchar(82)");

                    b.Property<int>("SudokuTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SudokuTypeId");

                    b.ToTable("Sudokus");
                });

            modelBuilder.Entity("Dictionary.Models.SudokuRecyclable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("SudokuId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SudokuId");

                    b.ToTable("SudokuRecyclable");
                });

            modelBuilder.Entity("Dictionary.Models.SudokuType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TypeName")
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id");

                    b.ToTable("SudokuType");
                });

            modelBuilder.Entity("Dictionary.Models.Word", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EndLetter")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<short>("Length")
                        .HasColumnType("smallint");

                    b.Property<string>("StartLetter")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("WordW")
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Length");

                    b.HasIndex("StartLetter");

                    b.HasIndex("WordW")
                        .IsUnique()
                        .HasFilter("[WordW] IS NOT NULL");

                    b.ToTable("Words");
                });

            modelBuilder.Entity("Dictionary.Models.Sudoku", b =>
                {
                    b.HasOne("Dictionary.Models.SudokuType", "SudokuType")
                        .WithMany()
                        .HasForeignKey("SudokuTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SudokuType");
                });

            modelBuilder.Entity("Dictionary.Models.SudokuRecyclable", b =>
                {
                    b.HasOne("Dictionary.Models.Sudoku", "Sudoku")
                        .WithMany()
                        .HasForeignKey("SudokuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sudoku");
                });
#pragma warning restore 612, 618
        }
    }
}
