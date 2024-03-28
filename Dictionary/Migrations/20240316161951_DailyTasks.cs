using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class DailyTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyTaskStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTaskStatuses", x => x.Id);
                });
            migrationBuilder.InsertData("DailyTaskStatuses", new string[] { "StatusId", "Name" }, new object[] { 1, "Not started" });
            migrationBuilder.InsertData("DailyTaskStatuses", new string[] { "StatusId", "Name" }, new object[] { 2, "Ongoing" });
            migrationBuilder.InsertData("DailyTaskStatuses", new string[] { "StatusId", "Name" }, new object[] { 3, "Done" });

            migrationBuilder.CreateTable(
                name: "DailyTaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", nullable: true),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTaskTypes", x => x.Id);
                });
            migrationBuilder.InsertData("DailyTaskTypes", new string[] { "TypeId", "Name", "Info" }, new object[] { 1, "Repeat", "Repeat the whole task every day." });
            migrationBuilder.InsertData("DailyTaskTypes", new string[] { "TypeId", "Name", "Info" }, new object[] { 2, "Evenly Distributed", "Split the task evenly as possible, then done within the whole task period." });

            migrationBuilder.CreateTable(
                name: "DailyTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(128)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DailyTaskStatusId = table.Column<int>(type: "int", nullable: false),
                    DailyTaskTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTasks_DailyTaskTypes_DailyTaskTypeId",
                        column: x => x.DailyTaskTypeId,
                        principalTable: "DailyTaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyTaskSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true),
                    DailyTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTaskSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTaskSchedules_DailyTasks_DailyTaskId",
                        column: x => x.DailyTaskId,
                        principalTable: "DailyTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyTaskSubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkLoad = table.Column<int>(type: "int", nullable: false),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true),
                    DailyTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTaskSubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTaskSubs_DailyTasks_DailyTaskId",
                        column: x => x.DailyTaskId,
                        principalTable: "DailyTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyTaskScheduleDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyTaskSubId = table.Column<int>(type: "int", nullable: false),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true),
                    DailyTaskScheduleId = table.Column<int>(type: "int", nullable: false),
                    DailyTaskStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTaskScheduleDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTaskScheduleDetails_DailyTaskSchedules_DailyTaskScheduleId",
                        column: x => x.DailyTaskScheduleId,
                        principalTable: "DailyTaskSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailyTaskScheduleDetails_DailyTaskStatuses_DailyTaskStatusId",
                        column: x => x.DailyTaskStatusId,
                        principalTable: "DailyTaskStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyTasks_DailyTaskTypeId",
                table: "DailyTasks",
                column: "DailyTaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTaskScheduleDetails_DailyTaskScheduleId",
                table: "DailyTaskScheduleDetails",
                column: "DailyTaskScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTaskScheduleDetails_DailyTaskStatusId",
                table: "DailyTaskScheduleDetails",
                column: "DailyTaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTaskSchedules_DailyTaskId",
                table: "DailyTaskSchedules",
                column: "DailyTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTaskSubs_DailyTaskId",
                table: "DailyTaskSubs",
                column: "DailyTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTaskScheduleDetails");

            migrationBuilder.DropTable(
                name: "DailyTaskSubs");

            migrationBuilder.DropTable(
                name: "DailyTaskSchedules");

            migrationBuilder.DropTable(
                name: "DailyTaskStatuses");

            migrationBuilder.DropTable(
                name: "DailyTasks");

            migrationBuilder.DropTable(
                name: "DailyTaskTypes");
        }
    }
}
