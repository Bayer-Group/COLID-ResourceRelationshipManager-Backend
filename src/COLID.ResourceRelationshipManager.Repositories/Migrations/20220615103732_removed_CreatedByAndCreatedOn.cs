using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public partial class removed_CreatedByAndCreatedOn : Migration
#pragma warning restore CA1707 // Identifiers should not contain underscores
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RelationMap");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "RelationMap");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RelationMap",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "RelationMap",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
