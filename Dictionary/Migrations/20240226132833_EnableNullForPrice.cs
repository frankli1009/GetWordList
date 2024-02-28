using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class EnableNullForPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "ConsumerGoodsDetails",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.CreateTable(
                name: "ConsumerGoodsExtra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Info = table.Column<string>(type: "varchar(254)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumerGoodsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerGoodsExtra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumerGoodsExtra_ConsumerGoods_ConsumerGoodsId",
                        column: x => x.ConsumerGoodsId,
                        principalTable: "ConsumerGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerGoodsExtra_ConsumerGoodsId",
                table: "ConsumerGoodsExtra",
                column: "ConsumerGoodsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumerGoodsExtra");

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "ConsumerGoodsDetails",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);
        }
    }
}
