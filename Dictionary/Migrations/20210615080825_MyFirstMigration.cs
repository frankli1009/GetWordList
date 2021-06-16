using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordW = table.Column<string>(type: "varchar(50)", nullable: true),
                    StartLetter = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    EndLetter = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Length = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Words_Length",
                table: "Words",
                column: "Length");

            migrationBuilder.CreateIndex(
                name: "IX_Words_StartLetter",
                table: "Words",
                column: "StartLetter");

            migrationBuilder.CreateIndex(
                name: "IX_Words_WordW",
                table: "Words",
                column: "WordW",
                unique: true,
                filter: "[WordW] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Words");
        }
    }
}
