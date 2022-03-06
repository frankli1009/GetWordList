using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AdjustSudokus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Sudokus",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<int>(
                name: "SudokuTypeId",
                table: "Sudokus",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "SudokuType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "varchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SudokuType", x => x.Id);
                });

            migrationBuilder.Sql(@"Insert Into dbo.SudokuType (TypeName) Values ('Easy')");
            migrationBuilder.Sql(@"Insert Into dbo.SudokuType (TypeName) Values ('Medium')");
            migrationBuilder.Sql(@"Insert Into dbo.SudokuType (TypeName) Values ('Difficult')");
                                          
            migrationBuilder.CreateIndex(
                name: "IX_Sudokus_SudokuTypeId",
                table: "Sudokus",
                column: "SudokuTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sudokus_SudokuType_SudokuTypeId",
                table: "Sudokus",
                column: "SudokuTypeId",
                principalTable: "SudokuType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sudokus_SudokuType_SudokuTypeId",
                table: "Sudokus");

            migrationBuilder.DropTable(
                name: "SudokuType");

            migrationBuilder.DropIndex(
                name: "IX_Sudokus_SudokuTypeId",
                table: "Sudokus");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Sudokus");

            migrationBuilder.DropColumn(
                name: "SudokuTypeId",
                table: "Sudokus");
        }
    }
}
