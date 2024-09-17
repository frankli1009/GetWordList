using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddDailyTaskTypeOpenMinded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData("DailyTaskTypes", new string[] { "TypeId", "Name", "Info" }, new object[] { 3, "Unrestricted", "Not really designated." });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("DailyTaskTypes", "TypeId", 3);
        }
    }
}
