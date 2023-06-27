using Microsoft.EntityFrameworkCore.Migrations;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public partial class updateDataTypeOfLinkinfo_Status : Migration
#pragma warning restore CA1707 // Identifiers should not contain underscores
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MapNodes",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MapLinkInfo",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MapNodes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MapLinkInfo",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
