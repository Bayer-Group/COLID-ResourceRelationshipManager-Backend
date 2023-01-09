using Microsoft.EntityFrameworkCore.Migrations;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
    public partial class descriptionColumnAddedInRelationMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_RelationMap_RelationMapId",
                table: "Nodes");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RelationMap",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PIDUri",
                table: "Nodes",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200) CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_RelationMap_RelationMapId",
                table: "Nodes",
                column: "RelationMapId",
                principalTable: "RelationMap",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_RelationMap_RelationMapId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RelationMap");

            migrationBuilder.AlterColumn<string>(
                name: "PIDUri",
                table: "Nodes",
                type: "varchar(200) CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_RelationMap_RelationMapId",
                table: "Nodes",
                column: "RelationMapId",
                principalTable: "RelationMap",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
