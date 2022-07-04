using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    public class GraphMap : ModifiedInformation
    {
        public Guid? GraphMapId { get; set; }
        public string Name { get; set; }
        public ICollection<MapNode> MapNodes { get; set; }
        public ICollection<MapLink> MapLinks { get; set; }
        public string Id { get; set; }
    }
}
