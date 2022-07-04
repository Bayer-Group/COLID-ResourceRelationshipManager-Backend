using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GraphMaps",
                columns: table => new
                {
                    GraphMapId = table.Column<Guid>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphMaps", x => x.GraphMapId);
                });

            migrationBuilder.CreateTable(
                name: "MapLinkType",
                columns: table => new
                {
                    MapLinkTypeId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapLinkType", x => x.MapLinkTypeId);
                });

            migrationBuilder.CreateTable(
                name: "MapNodes",
                columns: table => new
                {
                    MapNodeId = table.Column<Guid>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    LinkCount = table.Column<int>(nullable: false),
                    ResourceIdentifier = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ResourceType = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: true),
                    Fx = table.Column<int>(nullable: false),
                    Fy = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    PidUri = table.Column<string>(nullable: true),
                    GraphMapId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapNodes", x => x.MapNodeId);
                    table.ForeignKey(
                        name: "FK_MapNodes_GraphMaps_GraphMapId",
                        column: x => x.GraphMapId,
                        principalTable: "GraphMaps",
                        principalColumn: "GraphMapId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MapLinks",
                columns: table => new
                {
                    MapLinkId = table.Column<Guid>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    Id = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    Target = table.Column<string>(nullable: true),
                    NameMapLinkTypeId = table.Column<Guid>(nullable: true),
                    GraphMapId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapLinks", x => x.MapLinkId);
                    table.ForeignKey(
                        name: "FK_MapLinks_GraphMaps_GraphMapId",
                        column: x => x.GraphMapId,
                        principalTable: "GraphMaps",
                        principalColumn: "GraphMapId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapLinks_MapLinkType_NameMapLinkTypeId",
                        column: x => x.NameMapLinkTypeId,
                        principalTable: "MapLinkType",
                        principalColumn: "MapLinkTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MapLinkInfo",
                columns: table => new
                {
                    MapLinkInfoId = table.Column<Guid>(nullable: false),
                    TypeMapLinkTypeId = table.Column<Guid>(nullable: true),
                    StartNodeId = table.Column<string>(nullable: true),
                    EndNodeId = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    MapNodeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapLinkInfo", x => x.MapLinkInfoId);
                    table.ForeignKey(
                        name: "FK_MapLinkInfo_MapNodes_MapNodeId",
                        column: x => x.MapNodeId,
                        principalTable: "MapNodes",
                        principalColumn: "MapNodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapLinkInfo_MapLinkType_TypeMapLinkTypeId",
                        column: x => x.TypeMapLinkTypeId,
                        principalTable: "MapLinkType",
                        principalColumn: "MapLinkTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_MapNodeId",
                table: "MapLinkInfo",
                column: "MapNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_TypeMapLinkTypeId",
                table: "MapLinkInfo",
                column: "TypeMapLinkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinks_GraphMapId",
                table: "MapLinks",
                column: "GraphMapId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinks_NameMapLinkTypeId",
                table: "MapLinks",
                column: "NameMapLinkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MapNodes_GraphMapId",
                table: "MapNodes",
                column: "GraphMapId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapLinkInfo");

            migrationBuilder.DropTable(
                name: "MapLinks");

            migrationBuilder.DropTable(
                name: "MapNodes");

            migrationBuilder.DropTable(
                name: "MapLinkType");

            migrationBuilder.DropTable(
                name: "GraphMaps");
        }
    }
}
