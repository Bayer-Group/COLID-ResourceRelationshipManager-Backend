using Microsoft.EntityFrameworkCore.Migrations;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public partial class addedResourceType_In_NameValuePair : Migration
#pragma warning restore CA1707 // Identifiers should not contain underscores
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "NameValuePair",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "NameValuePair");
        }
    }
}
