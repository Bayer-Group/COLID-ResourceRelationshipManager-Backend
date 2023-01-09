﻿// <auto-generated />
using System;
using COLID.ResourceRelationshipManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
    [DbContext(typeof(ResourceRelationshipManagerContext))]
    partial class ResourceRelationshipManagerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.Entity.Nodes", b =>
                {
                    b.Property<Guid?>("NodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("PIDUri")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("RelationMapId")
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<double>("xPosition")
                        .HasColumnType("double");

                    b.Property<double>("yPosition")
                        .HasColumnType("double");

                    b.HasKey("NodeId");

                    b.HasIndex("RelationMapId");

                    b.ToTable("Nodes");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.Entity.RelationMap", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("RelationMap");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.GraphMap", b =>
                {
                    b.Property<Guid?>("GraphMapId")
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

                    b.Property<Guid?>("NameValuePairId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Source")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("SourceType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Target")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("TargetType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("MapLinkId");

                    b.HasIndex("GraphMapId");

                    b.HasIndex("NameValuePairId");

                    b.ToTable("MapLinks");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapLinkInfo", b =>
                {
                    b.Property<Guid>("MapLinkInfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("EndNodeNameValuePairId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("MapNodeId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("StartNodeNameValuePairId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Status")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("TypeNameValuePairId")
                        .HasColumnType("char(36)");

                    b.HasKey("MapLinkInfoId");

                    b.HasIndex("EndNodeNameValuePairId");

                    b.HasIndex("MapNodeId");

                    b.HasIndex("StartNodeNameValuePairId");

                    b.HasIndex("TypeNameValuePairId");

                    b.ToTable("MapLinkInfo");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapNode", b =>
                {
                    b.Property<Guid>("MapNodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<double>("Fx")
                        .HasColumnType("double");

                    b.Property<double>("Fy")
                        .HasColumnType("double");

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

                    b.Property<string>("Status")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("MapNodeId");

                    b.HasIndex("GraphMapId");

                    b.ToTable("MapNodes");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.NameValuePair", b =>
                {
                    b.Property<Guid>("NameValuePairId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ResourceType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Value")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("NameValuePairId");

                    b.ToTable("NameValuePair");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.Entity.Nodes", b =>
                {
                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.Entity.RelationMap", null)
                        .WithMany("Nodes")
                        .HasForeignKey("RelationMapId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapLink", b =>
                {
                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.GraphMap", null)
                        .WithMany("MapLinks")
                        .HasForeignKey("GraphMapId");

                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.NameValuePair", "Name")
                        .WithMany()
                        .HasForeignKey("NameValuePairId");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.MapLinkInfo", b =>
                {
                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.NameValuePair", "EndNode")
                        .WithMany()
                        .HasForeignKey("EndNodeNameValuePairId");

                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.MapNode", null)
                        .WithMany("Links")
                        .HasForeignKey("MapNodeId");

                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.NameValuePair", "StartNode")
                        .WithMany()
                        .HasForeignKey("StartNodeNameValuePairId");

                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.NameValuePair", "Type")
                        .WithMany()
                        .HasForeignKey("TypeNameValuePairId");
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
