using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddMonthParameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsumerGoodsParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDay = table.Column<int>(type: "int", nullable: false),
                    Params = table.Column<string>(type: "varchar(254)", nullable: true),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valid = table.Column<bool>(type: "bit", nullable: false),
                    ConsumerGoodsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerGoodsParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumerGoodsParameters_ConsumerGoods_ConsumerGoodsId",
                        column: x => x.ConsumerGoodsId,
                        principalTable: "ConsumerGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerGoodsParameters_ConsumerGoodsId",
                table: "ConsumerGoodsParameters",
                column: "ConsumerGoodsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumerGoodsParameters");
        }
    }
}
