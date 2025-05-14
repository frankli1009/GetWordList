using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AlterSPUpdateTaskStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"ALTER PROCEDURE [dbo].[spUpdateTaskStatus]
                    @taskId INT, @taskTypeId INT, @doneLeastWorkload INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @scheduleId INT
                    DECLARE cursorSchedules CURSOR
                        FOR
                            SELECT Id
                            From DailyTaskSchedules
                            WHERE DailyTaskId = @taskId;
                    OPEN cursorSchedules;
                    FETCH NEXT FROM cursorSchedules INTO @scheduleId;
                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        EXEC spUpdateTaskScheduleStatus @scheduleId, @taskTypeId, @doneLeastWorkload;

                        FETCH NEXT FROM cursorSchedules INTO @scheduleId;
                    END;
                    CLOSE cursorSchedules;
                    DEALLOCATE cursorSchedules;

                    --Here @schedulesCount only refers to those non-all-optional schedules (at least with one non-optional sub task in the schedule)
                    DECLARE @schedulesCount INT
                    --Here @doneSchedulesCount only refers to those non-all-optional schedules (at least with one non-optional sub task in the schedule)
                    DECLARE @doneSchedulesCount INT
                    DECLARE @startedSchedulesCount INT
                    SELECT @schedulesCount = COUNT(*)
                        FROM DailyTaskSchedules a
                        WHERE a.DailyTaskId = @taskId
                            AND EXISTS(SELECT 1 FROM DailyTaskScheduleDetails b, DailyTaskSubs c 
                                WHERE b.DailyTaskScheduleId = a.Id
                                AND b.DailyTaskSubId=c.Id AND c.Optional=0);
                    SELECT @doneSchedulesCount = COUNT(*)
                        FROM DailyTaskSchedules a
                        WHERE a.DailyTaskId = @taskId
                            AND EXISTS(SELECT 1 FROM DailyTaskScheduleDetails b, DailyTaskSubs c 
                                WHERE b.DailyTaskScheduleId = a.Id
                                AND b.DailyTaskSubId=c.Id AND c.Optional=0)
                            AND a.DailyTaskStatusId = 3;
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
                    UPDATE DailyTasks SET DailyTaskStatusId = @statusId
                        WHERE Id = @taskId;
                END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sp = @"ALTER PROCEDURE [dbo].[spUpdateTaskStatus]
                    @taskId INT, @taskTypeId INT, @doneLeastWorkload INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @scheduleId INT
                    DECLARE cursorSchedules CURSOR
                        FOR
                            SELECT Id
                            From DailyTaskSchedules
                            WHERE DailyTaskId = @taskId;
                    OPEN cursorSchedules;
                    FETCH NEXT FROM cursorSchedules INTO @scheduleId;
                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        EXEC spUpdateTaskScheduleStatus @scheduleId, @taskTypeId, @doneLeastWorkload;

                        FETCH NEXT FROM cursorSchedules INTO @scheduleId;
                    END;
                    CLOSE cursorSchedules;
                    DEALLOCATE cursorSchedules;

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
                    UPDATE DailyTasks SET DailyTaskStatusId = @statusId
                        WHERE Id = @taskId;
                END";
            migrationBuilder.Sql(sp);
        }
    }
}
