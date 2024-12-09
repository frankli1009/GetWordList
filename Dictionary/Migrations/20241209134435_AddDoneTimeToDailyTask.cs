using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddDoneTimeToDailyTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DoneTime",
                table: "DailyTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("UPDATE a SET DoneTime=a.EndDate"+
                " FROM DailyTasks a , (SELECT GetDate() AS CurDate) b" +
                " WHERE a.DailyTaskStatusId=3 AND a.EndDate < b.CurDate");
            migrationBuilder.Sql("UPDATE a SET DoneTime=b.CurDate" +
                " FROM DailyTasks a , (SELECT GetDate() AS CurDate) b" +
                " WHERE a.DailyTaskStatusId=3 AND a.EndDate >= b.CurDate");
            string sp = @"ALTER PROCEDURE [dbo].[spUpdateTaskStatusOnDetailStatusChanged]
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
                    IF (@statusId = 3)
                        UPDATE DailyTasks SET DailyTaskStatusId = @statusId, DoneTime = GetDate() WHERE Id = @taskId;
                    ELSE
                        UPDATE DailyTasks SET DailyTaskStatusId = @statusId, DoneTime = NULL WHERE Id = @taskId;
                END";
            migrationBuilder.Sql(sp);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp = @"ALTER PROCEDURE [dbo].[spUpdateTaskStatusOnDetailStatusChanged]
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

            migrationBuilder.DropColumn(
                name: "DoneTime",
                table: "DailyTasks");
        }
    }
}
