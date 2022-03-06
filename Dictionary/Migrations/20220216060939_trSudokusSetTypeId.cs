using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class trSudokusSetTypeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var tr = @"CREATE OR ALTER TRIGGER dbo.trSudokusSetTypeId   
                ON Sudokus   
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE Sudokus SET SudokuTypeId=(CASE
                        WHEN LEN(REPLACE(i.Data, '0', '')) >= 41 THEN 1
                        WHEN LEN(REPLACE(i.Data, '0', '')) BETWEEN 36 AND 40 THEN 2
                        ELSE 3 END)
                    FROM Sudokus s
                    INNER JOIN inserted i ON s.Id = i.Id
                END";

            migrationBuilder.Sql(tr);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER dbo.trSudokusSetTypeId");
        }
    }
}
