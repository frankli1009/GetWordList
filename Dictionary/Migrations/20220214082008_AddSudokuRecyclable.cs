using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddSudokuRecyclable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SudokuRecyclable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SudokuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SudokuRecyclable", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SudokuRecyclable_Sudokus_SudokuId",
                table: "SudokuRecyclable",
                column: "SudokuId",
                principalTable: "Sudokus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"Insert Into dbo.SudokuRecyclable (SudokuId) Values (4)");
            migrationBuilder.Sql(@"Insert Into dbo.SudokuRecyclable (SudokuId) Values (21)");
            migrationBuilder.Sql(@"Insert Into dbo.SudokuRecyclable (SudokuId) Values (416)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SudokuRecyclable_Sudokus_SudokuId",
                table: "SudokuRecyclable");

            migrationBuilder.DropTable(
                name: "SudokuRecyclable");
        }
    }
}
