using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class DeliveryManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(254)", nullable: true),
                    DeliveryNo = table.Column<string>(type: "varchar(254)", nullable: true),
                    OrderTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiveTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Info = table.Column<string>(type: "nvarchar(254)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryStatuses", x => x.Id);
                });

            migrationBuilder.InsertData("DeliveryStatuses", new string[] { "StatusId", "Name" }, new object[] { 1, "Ordered" });
            migrationBuilder.InsertData("DeliveryStatuses", new string[] { "StatusId", "Name" }, new object[] { 2, "Dispatched" });
            migrationBuilder.InsertData("DeliveryStatuses", new string[] { "StatusId", "Name" }, new object[] { 3, "Ontheway" });
            migrationBuilder.InsertData("DeliveryStatuses", new string[] { "StatusId", "Name" }, new object[] { 4, "Received" });
            migrationBuilder.InsertData("DeliveryStatuses", new string[] { "StatusId", "Name" }, new object[] { 5, "Cancelled" });

            migrationBuilder.InsertData("ToolKeyParams", new string[] { "Key", "Parameters", "Category" },
                new object[] { "MinimumCountToSplitIntoPages", "15", "Deliveries" });
            migrationBuilder.InsertData("ToolKeyParams", new string[] { "Key", "Parameters", "Category" },
                new object[] { "CountPerPage", "10", "Deliveries" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete From ToolKeyParams Where Category='Deliveries' and Key='MinimumCountToSplitIntoPages'");
            migrationBuilder.Sql("Delete From ToolKeyParams Where Category='Deliveries' and Key='CountPerPage'");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "DeliveryStatuses");
        }
    }
}
