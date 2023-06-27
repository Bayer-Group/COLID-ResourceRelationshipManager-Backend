
using System.Collections.Generic;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    public class ResourceCTO : COLID.Graph.Metadata.DataModels.Resources.Resource
    {
        public IList<dynamic> CustomLinks { get; set; }
    }
}
