using Microsoft.EntityFrameworkCore.Migrations;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
    public partial class addSource_TargetType : Migration
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
