using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COLID.ResourceRelationshipManager.Common.DataModels.Entity
{
    public class Nodes
    {
        [MaxLength(36)]
        public Guid NodeId { get; set; }
        public Uri PIDUri { get; set; }
        [ForeignKey("RelationMap")]
        [MaxLength(36)]
        public Guid RelationMapId { get; set; }
        public RelationMap RelationMap { get; set; }
        public double xPosition { get; set; }
        public double yPosition { get; set; }
    }
}
