using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COLID.ResourceRelationshipManager.Common.DataModels.Entity
{
    public class RelationMap : SharedMetadata
    {
        [MaxLength(36)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Nodes> Nodes { get; set; }

        [NotMapped]
        public int NodeCount => Nodes != null ? Convert.ToInt32($"{ Nodes.Count }") : 0;
    }
}
