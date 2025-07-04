using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddServiceFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UId = table.Column<string>(type: "varchar(64)", nullable: true),
                    ServiceName = table.Column<string>(type: "varchar(254)", nullable: true),
                    Category = table.Column<string>(type: "varchar(254)", nullable: true),
                    ServiceKey = table.Column<string>(type: "varchar(254)", nullable: true),
                    ServiceValue = table.Column<string>(type: "varchar(254)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(254)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceFileStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFileStatuses", x => x.Id);
                });
            migrationBuilder.Sql("Insert Into [dbo].[ServiceFileStatuses] (StatusId, Name) Values (1, 'ToUpload')");
            migrationBuilder.Sql("Insert Into [dbo].[ServiceFileStatuses] (StatusId, Name) Values (2, 'Uploaded')");
            migrationBuilder.Sql("Insert Into [dbo].[ServiceFileStatuses] (StatusId, Name) Values (3, 'Deleted')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceFiles");

            migrationBuilder.DropTable(
                name: "ServiceFileStatuses");
        }
    }
}
