using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddStatusIdToTaskSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyTaskStatusId",
                table: "DailyTaskSchedules",
                type: "int",
                nullable: false,
                defaultValue: 1);

            var sp = @"CREATE PROCEDURE [dbo].[spUpdateTaskScheduleStatus]
                    @scheduleId INT, @taskTypeId INT, @doneLeastWorkload INT
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
                            AND b.Optional = 0;
                    SELECT @optionalDoneCount = COUNT(*)
                        FROM DailyTaskScheduleDetails a, DailyTaskSubs b
                        WHERE a.DailyTaskSubId = b.ID 
                            AND a.DailyTaskScheduleId = @scheduleId AND a.DailyTaskStatusId = 3
                            AND b.Optional = 1;

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

            sp = @"CREATE PROCEDURE [dbo].[spUpdateTaskStatus]
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

            sp = @"CREATE PROCEDURE [dbo].[spUpdateAllTaskScheduleStatus]
                AS
                BEGIN
                    SET NOCOUNT ON;
                    DECLARE @taskId INT, @taskTypeId INT, @doneLeastWorkload INT;

                    DECLARE @Random INT;
                    DECLARE cursorTasks CURSOR
                        FOR
                            SELECT Id, DailyTaskTypeId, DoneLeastWorkload
                            FROM DailyTasks
                            ORDER BY Id;
                    OPEN cursorTasks;
                    FETCH NEXT FROM cursorTasks INTO @taskId, @taskTypeId, @doneLeastWorkload;
                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        EXEC spUpdateTaskStatus @taskId, @taskTypeId, @doneLeastWorkLoad;

                        FETCH NEXT FROM cursorTasks INTO @taskId, @taskTypeId, @doneLeastWorkload;
                    END;
                    CLOSE cursorTasks;
                    DEALLOCATE cursorTasks;
                END";
            migrationBuilder.Sql(sp);

            migrationBuilder.Sql("EXEC spUpdateAllTaskScheduleStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[spUpdateAllTaskScheduleStatus]");
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[spUpdateTaskStatus]");
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[spUpdateTaskScheduleStatus]");

            migrationBuilder.DropColumn(
                name: "DailyTaskStatusId",
                table: "DailyTaskSchedules");
        }
    }
}
