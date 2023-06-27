using System;
using System.Collections.Generic;
using System.Text;
using COLID.ResourceRelationshipManager.Common.DataModels;
using COLID.ResourceRelationshipManager.Common.DataModels.Entity;
using Microsoft.EntityFrameworkCore;

namespace COLID.ResourceRelationshipManager.Repositories
{
    public class ResourceRelationshipManagerContext : DbContext
    {
        public ResourceRelationshipManagerContext(DbContextOptions<ResourceRelationshipManagerContext> options) : base(options)
        {

        }
        public virtual DbSet<RelationMap> RelationMap { get; set; }
        public virtual DbSet<Node> Nodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RelationMap>().HasKey(m => m.Id);
            modelBuilder.Entity<Node>(entity =>
            {
                entity.HasKey(e => e.NodeId);
                entity.HasOne(n => n.RelationMap)
                    .WithMany(rm => rm.Nodes)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasForeignKey(n => n.RelationMapId);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

    }
}
