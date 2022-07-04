
using COLID.ResourceRelationshipManager.Common.DataModels.Link;
using System.Collections.Generic;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    public class ResourceCTO : COLID.Graph.Metadata.DataModels.Resources.Resource
    {
        public Dictionary<string, List<LinkingMapping>> Links { get; set; }
        public IList<dynamic> CustomLinks { get; set; }
    }
}
