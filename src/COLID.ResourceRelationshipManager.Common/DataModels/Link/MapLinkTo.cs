using System.Collections.Generic;

namespace COLID.ResourceRelationshipManager.Common.DataModels.Link
{
    public class MapLinkTo
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public KeyValuePair<string, string> LinkType { get; set; } // Key = identifier of the link type, value = name of the link type
    }
}
