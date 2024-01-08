using System;
using System.Collections.Generic;

namespace COLID.ResourceRelationshipManager.Common.DataModels.NewTOs
{
    public class GraphMapTO : SharedMetadata
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PidUri { get; set; }
        public IList<MapNodeTO> Nodes { get; set; }
    }
}
