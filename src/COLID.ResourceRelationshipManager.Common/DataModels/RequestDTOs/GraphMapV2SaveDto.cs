using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.DataModels.RequestDTOs
{
    public class GraphMapV2SaveDto
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public IList<NodeV2SaveDto> nodes { get; set; }
    }
}
