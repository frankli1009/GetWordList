using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddDoneWorkloadToTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoneLeastWorkload",
                table: "DailyTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoneLeastWorkload",
                table: "DailyTasks");
        }
    }
}
