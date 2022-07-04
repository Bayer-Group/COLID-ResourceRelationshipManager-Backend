using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
    public partial class addNameValuepairandRemoveLinkType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MapLinkInfo_MapLinkType_TypeMapLinkTypeId",
                table: "MapLinkInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_MapLinks_MapLinkType_NameMapLinkTypeId",
                table: "MapLinks");

            migrationBuilder.DropTable(
                name: "MapLinkType");

            migrationBuilder.DropIndex(
                name: "IX_MapLinks_NameMapLinkTypeId",
                table: "MapLinks");

            migrationBuilder.DropIndex(
                name: "IX_MapLinkInfo_TypeMapLinkTypeId",
                table: "MapLinkInfo");

            migrationBuilder.DropColumn(
                name: "NameMapLinkTypeId",
                table: "MapLinks");

            migrationBuilder.DropColumn(
                name: "EndNodeId",
                table: "MapLinkInfo");

            migrationBuilder.DropColumn(
                name: "StartNodeId",
                table: "MapLinkInfo");

            migrationBuilder.DropColumn(
                name: "TypeMapLinkTypeId",
                table: "MapLinkInfo");

            migrationBuilder.AddColumn<Guid>(
                name: "NameValuePairId",
                table: "MapLinks",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EndNodeNameValuePairId",
                table: "MapLinkInfo",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StartNodeNameValuePairId",
                table: "MapLinkInfo",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TypeNameValuePairId",
                table: "MapLinkInfo",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NameValuePair",
                columns: table => new
                {
                    NameValuePairId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameValuePair", x => x.NameValuePairId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapLinks_NameValuePairId",
                table: "MapLinks",
                column: "NameValuePairId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_EndNodeNameValuePairId",
                table: "MapLinkInfo",
                column: "EndNodeNameValuePairId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_StartNodeNameValuePairId",
                table: "MapLinkInfo",
                column: "StartNodeNameValuePairId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_TypeNameValuePairId",
                table: "MapLinkInfo",
                column: "TypeNameValuePairId");

            migrationBuilder.AddForeignKey(
                name: "FK_MapLinkInfo_NameValuePair_EndNodeNameValuePairId",
                table: "MapLinkInfo",
                column: "EndNodeNameValuePairId",
                principalTable: "NameValuePair",
                principalColumn: "NameValuePairId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MapLinkInfo_NameValuePair_StartNodeNameValuePairId",
                table: "MapLinkInfo",
                column: "StartNodeNameValuePairId",
                principalTable: "NameValuePair",
                principalColumn: "NameValuePairId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MapLinkInfo_NameValuePair_TypeNameValuePairId",
                table: "MapLinkInfo",
                column: "TypeNameValuePairId",
                principalTable: "NameValuePair",
                principalColumn: "NameValuePairId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MapLinks_NameValuePair_NameValuePairId",
                table: "MapLinks",
                column: "NameValuePairId",
                principalTable: "NameValuePair",
                principalColumn: "NameValuePairId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MapLinkInfo_NameValuePair_EndNodeNameValuePairId",
                table: "MapLinkInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_MapLinkInfo_NameValuePair_StartNodeNameValuePairId",
                table: "MapLinkInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_MapLinkInfo_NameValuePair_TypeNameValuePairId",
                table: "MapLinkInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_MapLinks_NameValuePair_NameValuePairId",
                table: "MapLinks");

            migrationBuilder.DropTable(
                name: "NameValuePair");

            migrationBuilder.DropIndex(
                name: "IX_MapLinks_NameValuePairId",
                table: "MapLinks");

            migrationBuilder.DropIndex(
                name: "IX_MapLinkInfo_EndNodeNameValuePairId",
                table: "MapLinkInfo");

            migrationBuilder.DropIndex(
                name: "IX_MapLinkInfo_StartNodeNameValuePairId",
                table: "MapLinkInfo");

            migrationBuilder.DropIndex(
                name: "IX_MapLinkInfo_TypeNameValuePairId",
                table: "MapLinkInfo");

            migrationBuilder.DropColumn(
                name: "NameValuePairId",
                table: "MapLinks");

            migrationBuilder.DropColumn(
                name: "EndNodeNameValuePairId",
                table: "MapLinkInfo");

            migrationBuilder.DropColumn(
                name: "StartNodeNameValuePairId",
                table: "MapLinkInfo");

            migrationBuilder.DropColumn(
                name: "TypeNameValuePairId",
                table: "MapLinkInfo");

            migrationBuilder.AddColumn<Guid>(
                name: "NameMapLinkTypeId",
                table: "MapLinks",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndNodeId",
                table: "MapLinkInfo",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartNodeId",
                table: "MapLinkInfo",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TypeMapLinkTypeId",
                table: "MapLinkInfo",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MapLinkType",
                columns: table => new
                {
                    MapLinkTypeId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Value = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapLinkType", x => x.MapLinkTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapLinks_NameMapLinkTypeId",
                table: "MapLinks",
                column: "NameMapLinkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_TypeMapLinkTypeId",
                table: "MapLinkInfo",
                column: "TypeMapLinkTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MapLinkInfo_MapLinkType_TypeMapLinkTypeId",
                table: "MapLinkInfo",
                column: "TypeMapLinkTypeId",
                principalTable: "MapLinkType",
                principalColumn: "MapLinkTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MapLinks_MapLinkType_NameMapLinkTypeId",
                table: "MapLinks",
                column: "NameMapLinkTypeId",
                principalTable: "MapLinkType",
                principalColumn: "MapLinkTypeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
