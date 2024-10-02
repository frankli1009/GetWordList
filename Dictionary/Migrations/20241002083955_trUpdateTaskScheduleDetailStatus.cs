using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class trUpdateTaskScheduleDetailStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE [dbo].[spUpdateScheduleStatusOnDetailStatusChanged]
                    @scheduleId INT, @insertedDetailId INT,
                    @insertedDetailStatus INT, @insertedDetailSubOptional INT,
                    @taskTypeId INT, @doneLeastWorkload INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @statusId INT;
                    DECLARE @nonOptionalCount INT, @nonOptionalDoneCount INT;
                    DECLARE @optionalDoneCount INT;

                    SELECT @nonOptionalCount = COUNT(*)
                        FROM DailyTaskScheduleDetails a, DailyTaskSubs b
                        WHERE a.DailyTaskSubId = b.ID 
                            AND a.DailyTaskScheduleId = @scheduleId
                            AND b.Optional = 0;

                    SELECT @nonOptionalDoneCount = COUNT(*)
                        FROM DailyTaskScheduleDetails a, DailyTaskSubs b
                        WHERE a.DailyTaskSubId = b.ID 
                            AND a.DailyTaskScheduleId = @scheduleId AND a.DailyTaskStatusId = 3
                            AND a.Id != @insertedDetailId
                            AND b.Optional = 0;
                    IF @insertedDetailSubOptional = 0 AND @insertedDetailStatus = 3
                    BEGIN
                        SET @nonOptionalDoneCount = @nonOptionalDoneCount + 1
                    END

                    SELECT @optionalDoneCount = COUNT(*)
                        FROM DailyTaskScheduleDetails a, DailyTaskSubs b
                        WHERE a.DailyTaskSubId = b.ID 
                            AND a.DailyTaskScheduleId = @scheduleId AND a.DailyTaskStatusId = 3
                            AND a.Id != @insertedDetailId
                            AND b.Optional = 1;
                    IF @insertedDetailSubOptional = 1 AND @insertedDetailStatus = 3
                    BEGIN
                        SET @optionalDoneCount = @optionalDoneCount + 1
                    END


                    DECLARE @doneWorkLoad INT
                    SELECT @doneWorkLoad = SUM(b.WorkLoad) FROM DailyTaskScheduleDetails a, DailyTaskSubs b
                        WHERE a.DailyTaskSubId = b.ID 
                            AND a.DailyTaskScheduleId = @scheduleId AND a.DailyTaskStatusId = 3;

                    IF (@taskTypeId != 3 AND @nonOptionalDoneCount = @nonOptionalCount AND @nonOptionalCount != 0)
                        SELECT @statusId = 3
                    ELSE
                    BEGIN
                        IF (@taskTypeId = 3 AND @nonOptionalDoneCount = @nonOptionalCount AND @doneWorkLoad >= @doneLeastWorkLoad)
                            SELECT @statusId = 3
                        ELSE
                        BEGIN
                            IF (@nonOptionalDoneCount > 0 OR @optionalDoneCount > 0)
                                SELECT @statusId = 2;
                            ELSE
                                SELECT @statusId = 1;
                        END;
                    END;
                    UPDATE DailyTaskSchedules SET DailyTaskStatusId = @statusId WHERE Id = @scheduleId;
                END";
            migrationBuilder.Sql(sp);

            sp = @"CREATE PROCEDURE [dbo].[spUpdateTaskStatusOnDetailStatusChanged]
                    @taskId INT, @insertedScheduleId INT, @insertedDetailId INT,
                    @insertedDetailStatus INT, @insertedDetailSubOptional INT,
                    @taskTypeId INT, @doneLeastWorkload INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    EXEC spUpdateScheduleStatusOnDetailStatusChanged @insertedScheduleId,
                        @insertedDetailId, @insertedDetailStatus, @insertedDetailSubOptional,
                        @taskTypeId, @doneLeastWorkload

                    DECLARE @doneSchedulesCount INT, @startedSchedulesCount INT;
                    DECLARE @schedulesCount INT
                    SELECT @schedulesCount = COUNT(*)
                        FROM DailyTaskSchedules a
                        WHERE a.DailyTaskId = @taskId AND
                            EXISTS(SELECT 1 FROM DailyTaskScheduleDetails b WHERE b.DailyTaskScheduleId = a.Id);
                    SELECT @doneSchedulesCount = COUNT(*)
                        FROM DailyTaskSchedules a
                        WHERE a.DailyTaskId = @taskId AND a.DailyTaskStatusId = 3;
                    SELECT @startedSchedulesCount = COUNT(*)
                        FROM DailyTaskSchedules a
                        WHERE a.DailyTaskId = @taskId AND a.DailyTaskStatusId = 2;

                    DECLARE @statusId INT;
                    IF ((@doneSchedulesCount = @schedulesCount) OR (@taskTypeId = 1 AND @doneSchedulesCount >= @doneLeastWorkload AND @doneLeastWorkload > 0))
                        SELECT @statusId = 3
                    ELSE
                    BEGIN
                        IF (@doneSchedulesCount > 0 OR @startedSchedulesCount > 0)
                            SELECT @statusId = 2;
                        ELSE
                            SELECT @statusId = 1;
                    END;
                    UPDATE DailyTasks SET DailyTaskStatusId = @statusId WHERE Id = @taskId;
                END";
            migrationBuilder.Sql(sp);

            var tr = @"CREATE OR ALTER TRIGGER [dbo].[trUpdateTaskScheduleDetailStatus]   
                    ON [dbo].[DailyTaskScheduleDetails]   
                    AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    IF UPDATE (DailyTaskStatusId)
                    BEGIN
                        DECLARE @taskId INT, @taskDetailId INT, @taskSubId INT;
                        DECLARE @taskScheduleId INT, @taskStatusId INT;
                        DECLARE @optional INT, @taskTypeId INT, @doneLeastWorkload INT;

                        DECLARE cursorTaskScheduleDetailInserted CURSOR
                            FOR
                                SELECT a.Id, a.DailyTaskScheduleId, a.DailyTaskStatusId, b.DailyTaskId, b.Optional
                                From inserted a, DailyTaskSubs b
                                WHERE a.DailyTaskSubId = b.Id;
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
                    END;
                END";

            migrationBuilder.Sql(tr);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER [dbo].[trUpdateTaskScheduleDetailStatus]");
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[spUpdateTaskStatusOnDetailStatusChanged]");
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[spUpdateScheduleStatusOnDetailStatusChanged]");
        }
    }
}
