using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class SplitServiceFileAndData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                table: "ServiceFiles");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ServiceFiles");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "ServiceFiles");

            migrationBuilder.DropColumn(
                name: "UploadTime",
                table: "ServiceFiles");

            migrationBuilder.CreateTable(
                name: "ServiceFileDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(254)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    UploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ServiceFileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFileDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceFileDatas_ServiceFiles_ServiceFileId",
                        column: x => x.ServiceFileId,
                        principalTable: "ServiceFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFileDatas_ServiceFileId",
                table: "ServiceFileDatas",
                column: "ServiceFileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceFileDatas");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "ServiceFiles",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ServiceFiles",
                type: "nvarchar(254)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "ServiceFiles",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadTime",
                table: "ServiceFiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
