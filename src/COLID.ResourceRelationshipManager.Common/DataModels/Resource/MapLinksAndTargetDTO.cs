using COLID.ResourceRelationshipManager.Common.DataModels.NewTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.DataModels.Resource
{
    public class MapLinksAndTargetDTO
    {
        public List<MapLinkTO> MapLinks { get; set; }
        public List<string> TargetURIs { get; set; }

    }
}
