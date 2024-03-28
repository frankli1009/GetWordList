using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AltertrUpdateConsumerGoodsDetailsInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var tr = @"CREATE OR ALTER TRIGGER dbo.trUpdateConsumerGoodsDetailsInfo   
                ON ConsumerGoodsDetails   
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @param1 FLOAT, @param2 FLOAT, @param3 FLOAT;
                    DECLARE @param VARCHAR(254);
                    DECLARE @paramIndex INT;
                    DECLARE @consumerGoodsId INT, @params VARCHAR(254);
                    DECLARE cursorParameters CURSOR
                        FOR
                            SELECT ConsumerGoodsId, Params
                            From ConsumerGoodsParameters
                            WHERE ConsumerGoodsId in (SELECT ConsumerGoodsId from inserted)
                                AND Valid = 1
                            ORDER BY ConsumerGoodsId;
                    OPEN cursorParameters;
                    FETCH NEXT FROM cursorParameters INTO @consumerGoodsId, @params;
                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        IF @consumerGoodsId = 1 
                        BEGIN
                            SET @paramIndex = 1;
                            DECLARE cursorParams CURSOR
                                FOR
                                    SELECT value FROM STRING_SPLIT(@params, ',');
                            OPEN cursorParams;
                            FETCH NEXT FROM cursorParams INTO @param;
                            WHILE @@FETCH_STATUS = 0
                            BEGIN
                                IF @paramIndex > 2 BREAK;
                                IF @paramIndex = 1 SET @param1 = CAST(@param AS FLOAT);
                                ELSE SET @param2 = CAST(@param AS FLOAT);
                                SET @paramIndex = @paramIndex + 1;
                                FETCH NEXT FROM cursorParams INTO @param;
                            END;
                            CLOSE cursorParams;
                            DEALLOCATE cursorParams;

                            UPDATE ConsumerGoodsDetails SET Info='MonthQuantity: ' + ltrim(str(i.Quantity-@param1)) + '; MonthPrice: ' + ltrim(str(i.Price-@param2))
                            FROM ConsumerGoodsDetails c
                            INNER JOIN inserted i ON c.Id = i.Id AND i.ConsumerGoodsId = @consumerGoodsId
                            WHERE c.Info IS NULL or TRIM(c.Info) = '';
                        END
                        ELSE IF @consumerGoodsId = 2
                        BEGIN
                            SET @paramIndex = 1;
                            DECLARE cursorParams CURSOR
                                FOR
                                    SELECT value FROM STRING_SPLIT(@params, ',');
                            OPEN cursorParams;
                            FETCH NEXT FROM cursorParams INTO @param;
                            WHILE @@FETCH_STATUS = 0
                            BEGIN
                                IF @paramIndex > 3 BREAK;
                                IF @paramIndex = 1 SET @param1 = CAST(@param AS FLOAT);
                                ELSE IF @paramIndex = 2 SET @param2 = CAST(@param AS FLOAT);
                                ELSE SET @param3 = CAST(@param AS FLOAT);
                                SET @paramIndex = @paramIndex + 1;
                                FETCH NEXT FROM cursorParams INTO @param;
                            END;
                            CLOSE cursorParams;
                            DEALLOCATE cursorParams;

                            UPDATE ConsumerGoodsDetails
                            SET Info='MonthQuantity: ' + ltrim(str(i.Quantity-@param1)) + '; MonthPrice: ' + (case when i.Price is null then ltrim(str((i.Quantity-@param1) / @param3)) else ltrim(str(i.Price-@param2)) end)
                            FROM ConsumerGoodsDetails c
                            INNER JOIN inserted i ON c.Id = i.Id AND i.ConsumerGoodsId = @consumerGoodsId
                            WHERE c.Info IS NULL or TRIM(c.Info) = '';
                        END;

                        FETCH NEXT FROM cursorParameters INTO @consumerGoodsId, @params;
                    END;
                    CLOSE cursorParameters;
                    DEALLOCATE cursorParameters;
                END";

            migrationBuilder.Sql(tr);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var tr = @"CREATE OR ALTER TRIGGER dbo.trUpdateConsumerGoodsDetailsInfo   
                ON ConsumerGoodsDetails   
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @param1 FLOAT, @param2 FLOAT, @param3 FLOAT;
                    DECLARE @param VARCHAR(254);
                    DECLARE @paramIndex INT;
                    DECLARE @consumerGoodsId INT, @params VARCHAR(254);
                    DECLARE cursorParameters CURSOR
                        FOR
                            SELECT ConsumerGoodsId, Params
                            From ConsumerGoodsParameters
                            WHERE ConsumerGoodsId in (SELECT ConsumerGoodsId from inserted)
                                AND Valid = 1
                            ORDER BY ConsumerGoodsId;
                    OPEN cursorParameters;
                    FETCH NEXT FROM cursorParameters INTO @consumerGoodsId, @params;
                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        IF @consumerGoodsId = 1 
                        BEGIN
                            SET @paramIndex = 1;
                            DECLARE cursorParams CURSOR
                                FOR
                                    SELECT value FROM STRING_SPLIT(@params, ',');
                            OPEN cursorParams;
                            FETCH NEXT FROM cursorParams INTO @param;
                            WHILE @@FETCH_STATUS = 0
                            BEGIN
                                IF @paramIndex > 2 BREAK;
                                IF @paramIndex = 1 SET @param1 = CAST(@param AS FLOAT);
                                ELSE SET @param2 = CAST(@param AS FLOAT);
                                SET @paramIndex = @paramIndex + 1;
                                FETCH NEXT FROM cursorParams INTO @param;
                            END;
                            CLOSE cursorParams;
                            DEALLOCATE cursorParams;

                            UPDATE ConsumerGoodsDetails SET Info='MonthQuantity: ' + ltrim(str(i.Quantity-@param1)) + '; MonthPrice: ' + ltrim(str(i.Price-@param2))
                            FROM ConsumerGoodsDetails c
                            INNER JOIN inserted i ON c.Id = i.Id AND i.ConsumerGoodsId = @consumerGoodsId
                            WHERE c.Info IS NULL;
                        END
                        ELSE IF @consumerGoodsId = 2
                        BEGIN
                            SET @paramIndex = 1;
                            DECLARE cursorParams CURSOR
                                FOR
                                    SELECT value FROM STRING_SPLIT(@params, ',');
                            OPEN cursorParams;
                            FETCH NEXT FROM cursorParams INTO @param;
                            WHILE @@FETCH_STATUS = 0
                            BEGIN
                                IF @paramIndex > 3 BREAK;
                                IF @paramIndex = 1 SET @param1 = CAST(@param AS FLOAT);
                                ELSE IF @paramIndex = 2 SET @param2 = CAST(@param AS FLOAT);
                                ELSE SET @param3 = CAST(@param AS FLOAT);
                                SET @paramIndex = @paramIndex + 1;
                                FETCH NEXT FROM cursorParams INTO @param;
                            END;
                            CLOSE cursorParams;
                            DEALLOCATE cursorParams;

                            UPDATE ConsumerGoodsDetails
                            SET Info='MonthQuantity: ' + ltrim(str(i.Quantity-@param1)) + '; MonthPrice: ' + (case when i.Price is null then ltrim(str((i.Quantity-@param1) / @param3)) else ltrim(str(i.Price-@param2)) end)
                            FROM ConsumerGoodsDetails c
                            INNER JOIN inserted i ON c.Id = i.Id AND i.ConsumerGoodsId = @consumerGoodsId
                            WHERE c.Info IS NULL;
                        END;

                        FETCH NEXT FROM cursorParameters INTO @consumerGoodsId, @params;
                    END;
                    CLOSE cursorParameters;
                    DEALLOCATE cursorParameters;
                END";

            migrationBuilder.Sql(tr);
        }
    }
}
