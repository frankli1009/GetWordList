using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictionary.Migrations
{
    public partial class AddLogLevelInOpLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OpLogLevelId",
                table: "OpLogs",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "OpLogLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpLogLevels", x => x.Id);
                });
            migrationBuilder.Sql("Insert Into OpLogLevels (LevelId, Name) Values (0, 'Info')");
            migrationBuilder.Sql("Insert Into OpLogLevels (LevelId, Name) Values (1, 'Success')");
            migrationBuilder.Sql("Insert Into OpLogLevels (LevelId, Name) Values (2, 'Warning')");
            migrationBuilder.Sql("Insert Into OpLogLevels (LevelId, Name) Values (3, 'Error')");
            migrationBuilder.Sql("Update a Set a.OpLogLevelId = b.Id from OpLogs a, OpLogLevels b Where a.OpLogLevelId is null and b.LevelId = 0");

            migrationBuilder.CreateIndex(
                name: "IX_OpLogs_OpLogLevelId",
                table: "OpLogs",
                column: "OpLogLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OpLogs_OpLogLevels_OpLogLevelId",
                table: "OpLogs",
                column: "OpLogLevelId",
                principalTable: "OpLogLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpLogs_OpLogLevels_OpLogLevelId",
                table: "OpLogs");

            migrationBuilder.DropTable(
                name: "OpLogLevels");

            migrationBuilder.DropIndex(
                name: "IX_OpLogs_OpLogLevelId",
                table: "OpLogs");

            migrationBuilder.DropColumn(
                name: "OpLogLevelId",
                table: "OpLogs");
        }
    }
}
