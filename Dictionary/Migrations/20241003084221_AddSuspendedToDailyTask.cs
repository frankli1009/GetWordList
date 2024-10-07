using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddSuspendedToDailyTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Suspended",
                table: "DailyTasks",
                type: "int",
                nullable: false,
                defaultValue: 0); // 1 : suspended, 2 : deleted

            migrationBuilder.CreateTable(
                name: "DailyTaskSuspendeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuspendedId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true),
                    DailyTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTaskSuspendeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTaskSuspendeds_DailyTasks_DailyTaskId",
                        column: x => x.DailyTaskId,
                        principalTable: "DailyTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyTaskSuspendeds_DailyTaskId",
                table: "DailyTaskSuspendeds",
                column: "DailyTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTaskSuspendeds");

            migrationBuilder.DropColumn(
                name: "Suspended",
                table: "DailyTasks");
        }
    }
}
