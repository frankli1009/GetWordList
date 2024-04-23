using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddToolKeyParams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToolKeyParams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "varchar(128)", nullable: true),
                    Parameters = table.Column<string>(type: "varchar(254)", nullable: true),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolKeyParams", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToolKeyParams");
        }
    }
}
