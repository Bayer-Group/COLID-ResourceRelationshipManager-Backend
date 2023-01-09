using System.Collections.Generic;

namespace COLID.ResourceRelationshipManager.Common.DataModels.NewTOs
{
    public class MapLinkTO
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public string SourceName { get; set; }
        public string SourceType { get; set; }
        public string TargetName { get; set; }
        public string TargetType { get; set; }
        public KeyValuePair<string, string> LinkType { get; set; } // Key = identifier of the link type, value = name of the link type
        public bool Outbound { get; set; }
        public bool IsVersionLink { get; set; }
    }
}
