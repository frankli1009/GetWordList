using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddConsumerTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsumerGoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(254)", nullable: true),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerGoods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerGoodsDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumerGoodsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerGoodsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumerGoodsDetails_ConsumerGoods_ConsumerGoodsId",
                        column: x => x.ConsumerGoodsId,
                        principalTable: "ConsumerGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerGoodsDetails_ConsumerGoodsId",
                table: "ConsumerGoodsDetails",
                column: "ConsumerGoodsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumerGoodsDetails");

            migrationBuilder.DropTable(
                name: "ConsumerGoods");
        }
    }
}
