using COLID.ResourceRelationshipManager.Common.DataModels.NewTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.DataModels.Resource
{
    public class MapLinksAndTargetDTO
    {
        public IList<MapLinkTO> MapLinks { get; set; }
        public IList<Uri> TargetURIs { get; set; }

    }
}
