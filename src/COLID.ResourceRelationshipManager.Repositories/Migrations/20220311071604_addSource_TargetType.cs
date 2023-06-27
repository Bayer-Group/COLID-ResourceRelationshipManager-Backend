using Microsoft.EntityFrameworkCore.Migrations;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public partial class addSource_TargetType : Migration
#pragma warning restore CA1707 // Identifiers should not contain underscores
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "MapLinks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetType",
                table: "MapLinks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "MapLinks");

            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "MapLinks");
        }
    }
}
