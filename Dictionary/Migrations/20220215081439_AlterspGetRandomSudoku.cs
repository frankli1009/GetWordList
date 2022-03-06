using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AlterspGetRandomSudoku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE OR ALTER PROCEDURE [dbo].[spGetRandomSudoku]
                    @oldId INT, @sType VARCHAR(20)
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @TypeId INT;
                    SELECT @TypeId = 1;
                    IF EXISTS(SELECT Id FROM SudokuType WHERE LOWER(TypeName) = LOWER(@sType))
                    BEGIN
                        SELECT @TypeId = Id FROM SudokuType WHERE LOWER(TypeName) = LOWER(@sType)
                    END

                    DECLARE @Count INT;
                    SELECT @Count=COUNT(*) FROM Sudokus WHERE SudokuTypeId = @TypeId;
                    DECLARE @Lower INT;
                    SELECT @Lower = 1;
                    DECLARE @Upper INT;
                    SELECT @Upper = @Count;

                    DECLARE @Id INT;
                    SELECT @Id = @oldId;
                    WHILE @oldId = @Id
                    BEGIN
                        DECLARE @Random INT;
                        SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0)

                        SELECT @Id = Id FROM 
                        (SELECT ROW_NUMBER() OVER(ORDER BY Id ASC) AS row#, * FROM Sudokus WHERE SudokuTypeId = @TypeId) t
                        WHERE t.row#=@Random
                    END

                    SELECT * FROM Sudokus WHERE Id = @Id
                END";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE OR ALTER PROCEDURE [dbo].[spGetRandomSudoku]
                    @oldId int
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @Lower INT;
                    SELECT @Lower = Min(ID) FROM SUDOKUS;
                    DECLARE @Upper INT;
                    SELECT @Upper = Max(ID) FROM SUDOKUS;

                    DECLARE @Random INT;
                    SELECT @Random = 0;
                    WHILE @oldId = @Random
                    BEGIN
                        SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0)
                    END

                    WHILE  not EXISTS (SELECT * FROM SUDOKUS WHERE ID = @Random)
                    BEGIN
                        SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0)
                        WHILE @oldId = @Random
                        BEGIN
                            SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0)
                        END
                    END

                    SELECT * FROM SUDOKUS WHERE ID = @Random
                END";

            migrationBuilder.Sql(sp);
        }
    }
}
