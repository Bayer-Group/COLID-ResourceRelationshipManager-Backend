﻿// <auto-generated />
using System;
using COLID.ResourceRelationshipManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
    [DbContext(typeof(ResourceRelationshipManagerContext))]
    [Migration("20210922133014_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.GraphMap", b =>
                {
                    b.Property<Guid>("GraphMapId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Id")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("GraphMapId");

                    b.ToTable("GraphMaps");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapLink", b =>
                {
                    b.Property<Guid>("MapLinkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("GraphMapId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Id")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("NameMapLinkTypeId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Source")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Target")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("MapLinkId");

                    b.HasIndex("GraphMapId");

                    b.HasIndex("NameMapLinkTypeId");

                    b.ToTable("MapLinks");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapLinkInfo", b =>
                {
                    b.Property<Guid>("MapLinkInfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("EndNodeId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("MapNodeId")
                        .HasColumnType("char(36)");

                    b.Property<string>("StartNodeId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid?>("TypeMapLinkTypeId")
                        .HasColumnType("char(36)");

                    b.HasKey("MapLinkInfoId");

                    b.HasIndex("MapNodeId");

                    b.HasIndex("TypeMapLinkTypeId");

                    b.ToTable("MapLinkInfo");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapLinkType", b =>
                {
                    b.Property<Guid>("MapLinkTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Value")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("MapLinkTypeId");

                    b.ToTable("MapLinkType");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapNode", b =>
                {
                    b.Property<Guid>("MapNodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Fx")
                        .HasColumnType("int");

                    b.Property<int>("Fy")
                        .HasColumnType("int");

                    b.Property<Guid?>("GraphMapId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Id")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<int>("LinkCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PidUri")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ResourceIdentifier")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ResourceType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ShortName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("MapNodeId");

                    b.HasIndex("GraphMapId");

                    b.ToTable("MapNodes");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapLink", b =>
                {
                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.GraphMap", null)
                        .WithMany("MapLinks")
                        .HasForeignKey("GraphMapId");

                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.MapLinkType", "Name")
                        .WithMany()
                        .HasForeignKey("NameMapLinkTypeId");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapLinkInfo", b =>
                {
                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.MapNode", null)
                        .WithMany("Links")
                        .HasForeignKey("MapNodeId");

                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.MapLinkType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeMapLinkTypeId");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapNode", b =>
                {
                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.GraphMap", null)
                        .WithMany("MapNodes")
                        .HasForeignKey("GraphMapId");
                });
#pragma warning restore 612, 618
        }
    }
}
