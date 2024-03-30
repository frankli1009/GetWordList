using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AlterInfoOfTaskSubToNVAR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Info",
                table: "DailyTaskSubs",
                type: "nvarchar(254)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(254)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Info",
                table: "DailyTaskSubs",
                type: "varchar(254)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(254)",
                oldNullable: true);
        }
    }
}
