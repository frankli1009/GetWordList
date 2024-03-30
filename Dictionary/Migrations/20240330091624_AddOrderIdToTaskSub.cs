using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddOrderIdToTaskSub : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "DailyTaskSubs",
                type: "int",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql("Update t1 Set t1.OrderId = t2.OrderInTask From DailyTaskSubs t1, " +
                "(SELECT c.Id, ROW_NUMBER() OVER (PARTITION BY c.DailyTaskId ORDER BY c.Id) as OrderInTask FROM DailyTaskSubs c) t2 "+
                "Where t1.Id=t2.Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "DailyTaskSubs");
        }
    }
}
