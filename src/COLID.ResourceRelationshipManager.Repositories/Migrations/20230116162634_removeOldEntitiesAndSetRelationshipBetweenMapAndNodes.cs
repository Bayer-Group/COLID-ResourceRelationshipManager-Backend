using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
    public partial class removeOldEntitiesAndSetRelationshipBetweenMapAndNodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapLinkInfo");

            migrationBuilder.DropTable(
                name: "MapLinks");

            migrationBuilder.DropTable(
                name: "MapNodes");

            migrationBuilder.DropTable(
                name: "NameValuePair");

            migrationBuilder.DropTable(
                name: "GraphMaps");

            migrationBuilder.AlterColumn<Guid>(
                name: "RelationMapId",
                table: "Nodes",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldMaxLength: 36,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "RelationMapId",
                table: "Nodes",
                type: "char(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(Guid),
                oldMaxLength: 36);

            migrationBuilder.CreateTable(
                name: "GraphMaps",
                columns: table => new
                {
                    GraphMapId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Id = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphMaps", x => x.GraphMapId);
                });

            migrationBuilder.CreateTable(
                name: "NameValuePair",
                columns: table => new
                {
                    NameValuePairId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ResourceType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Value = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameValuePair", x => x.NameValuePairId);
                });

            migrationBuilder.CreateTable(
                name: "MapNodes",
                columns: table => new
                {
                    MapNodeId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Fx = table.Column<double>(type: "double", nullable: false),
                    Fy = table.Column<double>(type: "double", nullable: false),
                    GraphMapId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Id = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Index = table.Column<int>(type: "int", nullable: false),
                    LinkCount = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PidUri = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ResourceIdentifier = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ResourceType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ShortName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
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
                    MapLinkId = table.Column<Guid>(type: "char(36)", nullable: false),
                    GraphMapId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Id = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    NameValuePairId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Source = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    SourceType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Target = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    TargetType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
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
                        name: "FK_MapLinks_NameValuePair_NameValuePairId",
                        column: x => x.NameValuePairId,
                        principalTable: "NameValuePair",
                        principalColumn: "NameValuePairId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MapLinkInfo",
                columns: table => new
                {
                    MapLinkInfoId = table.Column<Guid>(type: "char(36)", nullable: false),
                    EndNodeNameValuePairId = table.Column<Guid>(type: "char(36)", nullable: true),
                    MapNodeId = table.Column<Guid>(type: "char(36)", nullable: true),
                    StartNodeNameValuePairId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    TypeNameValuePairId = table.Column<Guid>(type: "char(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapLinkInfo", x => x.MapLinkInfoId);
                    table.ForeignKey(
                        name: "FK_MapLinkInfo_NameValuePair_EndNodeNameValuePairId",
                        column: x => x.EndNodeNameValuePairId,
                        principalTable: "NameValuePair",
                        principalColumn: "NameValuePairId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapLinkInfo_MapNodes_MapNodeId",
                        column: x => x.MapNodeId,
                        principalTable: "MapNodes",
                        principalColumn: "MapNodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapLinkInfo_NameValuePair_StartNodeNameValuePairId",
                        column: x => x.StartNodeNameValuePairId,
                        principalTable: "NameValuePair",
                        principalColumn: "NameValuePairId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapLinkInfo_NameValuePair_TypeNameValuePairId",
                        column: x => x.TypeNameValuePairId,
                        principalTable: "NameValuePair",
                        principalColumn: "NameValuePairId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_EndNodeNameValuePairId",
                table: "MapLinkInfo",
                column: "EndNodeNameValuePairId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_MapNodeId",
                table: "MapLinkInfo",
                column: "MapNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_StartNodeNameValuePairId",
                table: "MapLinkInfo",
                column: "StartNodeNameValuePairId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinkInfo_TypeNameValuePairId",
                table: "MapLinkInfo",
                column: "TypeNameValuePairId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinks_GraphMapId",
                table: "MapLinks",
                column: "GraphMapId");

            migrationBuilder.CreateIndex(
                name: "IX_MapLinks_NameValuePairId",
                table: "MapLinks",
                column: "NameValuePairId");

            migrationBuilder.CreateIndex(
                name: "IX_MapNodes_GraphMapId",
                table: "MapNodes",
                column: "GraphMapId");
        }
    }
}
