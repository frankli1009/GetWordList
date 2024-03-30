using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddOrderIdToTaskScheduleDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "DailyTaskScheduleDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql("Update t1 Set t1.OrderId = t2.OrderInTaskSchedule From DailyTaskScheduleDetails t1, " +
                "(SELECT c.Id, ROW_NUMBER() OVER (PARTITION BY c.DailyTaskScheduleId ORDER BY c.Id) as OrderInTaskSchedule FROM DailyTaskScheduleDetails c) t2 " +
                "Where t1.Id=t2.Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "DailyTaskScheduleDetails");
        }
    }
}
