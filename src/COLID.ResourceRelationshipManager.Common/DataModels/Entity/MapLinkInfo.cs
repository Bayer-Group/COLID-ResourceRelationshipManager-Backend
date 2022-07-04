using COLID.ResourceRelationshipManager.Common.DataModels.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    public class MapLinkInfo
    {
        public Guid MapLinkInfoId { get; set; }
        public NameValuePair Type { get; set; }
        public Status Status { get; set; }
        public NameValuePair StartNode { get; set; }
        public NameValuePair EndNode { get; set; }
    }
}
