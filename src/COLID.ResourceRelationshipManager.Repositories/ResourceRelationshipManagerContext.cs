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

        public virtual DbSet<GraphMap> GraphMaps { get; set; }
        public virtual DbSet<MapNode> MapNodes { get; set; }
        public virtual DbSet<MapLink> MapLinks { get; set; }
        public virtual DbSet<RelationMap> RelationMap { get; set; }
        public virtual DbSet<Nodes> Nodes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GraphMap>().HasKey(m => m.GraphMapId);
            modelBuilder.Entity<MapNode>().HasKey(m => m.MapNodeId);
            modelBuilder.Entity<MapLink>().HasKey(m => m.MapLinkId);
            modelBuilder.Entity<MapLinkInfo>().HasKey(m => m.MapLinkInfoId);
            modelBuilder.Entity<NameValuePair>().HasKey(m => m.NameValuePairId);
            modelBuilder.Entity<RelationMap>().HasKey(m => m.Id);
            modelBuilder.Entity<Nodes>().HasKey(m => m.NodeId);

            modelBuilder.Entity<MapLinkInfo>()
                .Property(x => x.Status)
                .HasConversion(v => v.Value, v => new Status(v));

            modelBuilder.Entity<MapNode>()
               .Property(x => x.Status)
               .HasConversion(v => v.Value, v => new Status(v));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

    }
}
