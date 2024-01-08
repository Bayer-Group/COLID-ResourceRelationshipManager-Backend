﻿// <auto-generated />
using System;
using COLID.ResourceRelationshipManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace COLID.ResourceRelationshipManager.Repositories.Migrations
{
    [DbContext(typeof(ResourceRelationshipManagerContext))]
    [Migration("20230912102515_addedPidUriToRelationMap")]
    partial class addedPidUriToRelationMap
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.Entity.Node", b =>
                {
                    b.Property<Guid>("NodeId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("char(36)");

                    b.Property<string>("PIDUri")
                        .HasColumnType("longtext");

                    b.Property<Guid>("RelationMapId")
                        .HasMaxLength(36)
                        .HasColumnType("char(36)");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("char(36)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("PidUri")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("RelationMap");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.Entity.Node", b =>
                {
                    b.HasOne("COLID.ResourceRelationshipManager.Common.DataModels.Entity.RelationMap", "RelationMap")
                        .WithMany("Nodes")
                        .HasForeignKey("RelationMapId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RelationMap");
                });

            modelBuilder.Entity("COLID.ResourceRelationshipManager.Common.DataModels.Entity.RelationMap", b =>
                {
                    b.Navigation("Nodes");
                });
#pragma warning restore 612, 618
        }
    }
}
