using COLID.ResourceRelationshipManager.Common.DataModels.Entity;
using System;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    public class MapLink : ModifiedInformation
    {
        public Guid MapLinkId { get; set; }
        public string Id { get; set; }
        public Uri Source { get; set; }
        public Uri Target { get; set; }
        public string SourceType { get; set; }
        public string TargetType { get; set; }
        public NameValuePair Name { get; set; }
        public Status Status { get; set; }
    }
}
