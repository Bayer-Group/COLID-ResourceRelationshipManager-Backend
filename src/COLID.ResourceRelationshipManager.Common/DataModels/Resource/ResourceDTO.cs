using COLID.Graph.Metadata.DataModels.Resources;
using System;
using System.Collections.Generic;


namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    public class ResourceDTO
    {
        public string ResourceIdentifier { get; set; }
        public string ResourceType { get; set; }
        public string Name { get; set; }
        public IList<MapLinkInfo> Links { get; set; }
        public IList<VersionOverviewCTO> Versions { get; set; }
        public VersionOverviewCTO PreviousVersion { get; set; }
        public VersionOverviewCTO LaterVersion { get; set; }
        public Uri PidUri { get; set; }
    }
}
