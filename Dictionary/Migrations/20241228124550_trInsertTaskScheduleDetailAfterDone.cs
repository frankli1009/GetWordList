using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class trInsertTaskScheduleDetailAfterDone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var tr = @"CREATE OR ALTER TRIGGER [dbo].[trUInsertTaskScheduleDetailAfterDone]   
                    ON [dbo].[DailyTaskScheduleDetails]   
                    AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @taskId INT, @taskDetailId INT, @taskSubId INT;
                    DECLARE @taskScheduleId INT, @taskStatusId INT;
                    DECLARE @optional INT, @taskTypeId INT, @doneLeastWorkload INT;

                    DECLARE cursorTaskScheduleDetailInserted CURSOR
                        FOR
                            SELECT a.Id, a.DailyTaskScheduleId, a.DailyTaskStatusId, b.DailyTaskId, b.Optional
                            From inserted a, DailyTaskSubs b, DailyTasks c
                            WHERE a.DailyTaskSubId = b.Id and b.DailyTaskId = c.Id and c.DailyTaskStatusId = 3;
                    OPEN cursorTaskScheduleDetailInserted;
                    FETCH NEXT FROM cursorTaskScheduleDetailInserted INTO @taskDetailId,
                        @taskScheduleId, @taskStatusId, @taskId, @optional;
                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        SELECT @taskTypeId = DailyTaskTypeId, @doneLeastWorkload = DoneLeastWorkLoad
                            FROM DailyTasks WHERE Id = @taskId;

                        EXEC spUpdateTaskStatusOnDetailStatusChanged @taskId,
                            @taskScheduleId, @taskDetailId, @taskStatusId,
                            @optional, @taskTypeId, @doneLeastWorkload;
                        
                        FETCH NEXT FROM cursorTaskScheduleDetailInserted INTO @taskDetailId,
                            @taskScheduleId, @taskStatusId, @taskId, @optional;
                    END;
                    CLOSE cursorTaskScheduleDetailInserted;
                    DEALLOCATE cursorTaskScheduleDetailInserted;
                END";

            migrationBuilder.Sql(tr);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER [dbo].[trUInsertTaskScheduleDetailAfterDone]");
        }
    }
}
