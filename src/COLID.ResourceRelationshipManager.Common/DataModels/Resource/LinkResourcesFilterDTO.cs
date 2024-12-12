using COLID.ResourceRelationshipManager.Common.DataModels.NewTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.DataModels.Resource
{
    public enum FilterType
    {
        LinkType,
        ResourceType,
        Outbound
    }
    public class LinkResourcesFilterDTO
    {
        public IList<Uri> PIDUris { get; set; }
        public FilterType FilterType { get; set; }   // Please keep values as [linkType, resourceType, outbound]
        public string FilterValue { get; set; } 

    }
}
