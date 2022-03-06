using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class spGetRandomSudoku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE [dbo].[spGetRandomSudoku]
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE [dbo].[spGetRandomSudoku]");
        }
    }
}
