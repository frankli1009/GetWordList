﻿// <auto-generated />
using System;
using Dictionary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dictionary.Migrations
{
    [DbContext(typeof(WordDbContext))]
    partial class WordDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Dictionary.Models.ConsumerGoods", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Info")
                        .HasColumnType("varchar(254)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(254)");

                    b.HasKey("Id");

                    b.ToTable("ConsumerGoods");
                });

            modelBuilder.Entity("Dictionary.Models.ConsumerGoodsDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConsumerGoodsId")
                        .HasColumnType("int");

                    b.Property<string>("Info")
                        .HasColumnType("varchar(254)");

                    b.Property<float?>("Price")
                        .HasColumnType("real");

                    b.Property<float>("Quantity")
                        .HasColumnType("real");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ConsumerGoodsId");

                    b.ToTable("ConsumerGoodsDetails");
                });

            modelBuilder.Entity("Dictionary.Models.ConsumerGoodsExtra", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConsumerGoodsId")
                        .HasColumnType("int");

                    b.Property<string>("Info")
                        .HasColumnType("varchar(254)");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ConsumerGoodsId");

                    b.ToTable("ConsumerGoodsExtra");
                });

            modelBuilder.Entity("Dictionary.Models.ConsumerGoodsParameters", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConsumerGoodsId")
                        .HasColumnType("int");

                    b.Property<string>("Info")
                        .HasColumnType("varchar(254)");

                    b.Property<string>("Params")
                        .HasColumnType("varchar(254)");

                    b.Property<int>("StartDay")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Valid")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ConsumerGoodsId");

                    b.ToTable("ConsumerGoodsParameters");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DailyTaskStatusId")
                        .HasColumnType("int");

                    b.Property<int>("DailyTaskTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Info")
                        .HasColumnType("varchar(254)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(128)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DailyTaskTypeId");

                    b.ToTable("DailyTasks");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTaskSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ActDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DailyTaskId")
                        .HasColumnType("int");

                    b.Property<string>("Info")
                        .HasColumnType("varchar(254)");

                    b.HasKey("Id");

                    b.HasIndex("DailyTaskId");

                    b.ToTable("DailyTaskSchedules");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTaskScheduleDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DailyTaskScheduleId")
                        .HasColumnType("int");

                    b.Property<int>("DailyTaskStatusId")
                        .HasColumnType("int");

                    b.Property<int>("DailyTaskSubId")
                        .HasColumnType("int");

                    b.Property<string>("Info")
                        .HasColumnType("varchar(254)");

                    b.HasKey("Id");

                    b.HasIndex("DailyTaskScheduleId");

                    b.HasIndex("DailyTaskStatusId");

                    b.ToTable("DailyTaskScheduleDetails");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTaskStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(64)");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("DailyTaskStatuses");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTaskSub", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DailyTaskId")
                        .HasColumnType("int");

                    b.Property<string>("Info")
                        .HasColumnType("varchar(254)");

                    b.Property<int>("WorkLoad")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DailyTaskId");

                    b.ToTable("DailyTaskSubs");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTaskType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Info")
                        .HasColumnType("varchar(254)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(128)");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("DailyTaskTypes");
                });

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

            modelBuilder.Entity("Dictionary.Models.ConsumerGoodsDetail", b =>
                {
                    b.HasOne("Dictionary.Models.ConsumerGoods", "ConsumerGoods")
                        .WithMany()
                        .HasForeignKey("ConsumerGoodsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ConsumerGoods");
                });

            modelBuilder.Entity("Dictionary.Models.ConsumerGoodsExtra", b =>
                {
                    b.HasOne("Dictionary.Models.ConsumerGoods", "ConsumerGoods")
                        .WithMany()
                        .HasForeignKey("ConsumerGoodsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ConsumerGoods");
                });

            modelBuilder.Entity("Dictionary.Models.ConsumerGoodsParameters", b =>
                {
                    b.HasOne("Dictionary.Models.ConsumerGoods", "ConsumerGoods")
                        .WithMany()
                        .HasForeignKey("ConsumerGoodsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ConsumerGoods");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTask", b =>
                {
                    b.HasOne("Dictionary.Models.DailyTaskType", "DailyTaskType")
                        .WithMany()
                        .HasForeignKey("DailyTaskTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DailyTaskType");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTaskSchedule", b =>
                {
                    b.HasOne("Dictionary.Models.DailyTask", "DailyTask")
                        .WithMany()
                        .HasForeignKey("DailyTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DailyTask");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTaskScheduleDetail", b =>
                {
                    b.HasOne("Dictionary.Models.DailyTaskSchedule", "DailyTaskSchedule")
                        .WithMany()
                        .HasForeignKey("DailyTaskScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dictionary.Models.DailyTaskStatus", "DailyTaskStatus")
                        .WithMany()
                        .HasForeignKey("DailyTaskStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DailyTaskSchedule");

                    b.Navigation("DailyTaskStatus");
                });

            modelBuilder.Entity("Dictionary.Models.DailyTaskSub", b =>
                {
                    b.HasOne("Dictionary.Models.DailyTask", "DailyTask")
                        .WithMany()
                        .HasForeignKey("DailyTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DailyTask");
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
