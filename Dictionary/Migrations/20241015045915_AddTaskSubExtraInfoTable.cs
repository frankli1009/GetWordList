using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddTaskSubExtraInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyTaskSubExtraInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Info = table.Column<string>(type: "nvarchar(254)", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    DailyTaskId = table.Column<int>(type: "int", nullable: false),
                    DailyTaskSubId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTaskSubExtraInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyTaskSubExtraInfos_DailyTaskSubs_DailyTaskSubId",
                        column: x => x.DailyTaskSubId,
                        principalTable: "DailyTaskSubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyTaskSubExtraInfos_DailyTaskSubId",
                table: "DailyTaskSubExtraInfos",
                column: "DailyTaskSubId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTaskSubExtraInfos");
        }
    }
}
