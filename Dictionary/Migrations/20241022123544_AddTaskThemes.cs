using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddTaskThemes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyTaskThemeId",
                table: "DailyTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DailyTaskThemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThemeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", nullable: true),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true),
                    DefaultDailyTaskTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTaskThemes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyTasks_DailyTaskThemeId",
                table: "DailyTasks",
                column: "DailyTaskThemeId");

            migrationBuilder.Sql("Insert Into [dbo].[DailyTaskThemes] (ThemeId, Name, DefaultDailyTaskTypeId) Values (1, 'Reading', 2)");
            migrationBuilder.Sql("Insert Into [dbo].[DailyTaskThemes] (ThemeId, Name, DefaultDailyTaskTypeId) Values (2, 'Music', 1)");
            migrationBuilder.Sql("Insert Into [dbo].[DailyTaskThemes] (ThemeId, Name, DefaultDailyTaskTypeId) Values (3, 'Activities', 3)");

            migrationBuilder.Sql("update dt set DailyTaskThemeId=1, Name=replace(Name, 'Reading-', '') from dailytasks dt where dt.Name like 'Reading-%'");
            migrationBuilder.Sql("update dt set DailyTaskThemeId=2 from dailytasks dt where dt.Name like '%Piano%'");
            migrationBuilder.Sql("update dt set DailyTaskThemeId=3 from dailytasks dt where dt.DailyTaskThemeId=0");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyTasks_DailyTaskThemes_DailyTaskThemeId",
                table: "DailyTasks",
                column: "DailyTaskThemeId",
                principalTable: "DailyTaskThemes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyTasks_DailyTaskThemes_DailyTaskThemeId",
                table: "DailyTasks");

            migrationBuilder.DropTable(
                name: "DailyTaskThemes");

            migrationBuilder.DropIndex(
                name: "IX_DailyTasks_DailyTaskThemeId",
                table: "DailyTasks");

            migrationBuilder.DropColumn(
                name: "DailyTaskThemeId",
                table: "DailyTasks");
        }
    }
}
