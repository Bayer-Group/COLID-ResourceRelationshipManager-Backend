using COLID.ResourceRelationshipManager.Common.DataModels.Entity;
using System;
using System.Collections.Generic;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    public class MapNode : ModifiedInformation
    {
        public Guid MapNodeId { get; set; }
        public ICollection<MapLinkInfo> Links { get; set; }
        public int LinkCount { get; set; }
        public string ResourceIdentifier { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public string ResourceType { get; set; }
        public Uri Id { get; set; }
        public double Fx { get; set; }
        public double Fy { get; set; }
        public int Index { get; set; }
        public Uri PidUri { get; set; }
    }
}

