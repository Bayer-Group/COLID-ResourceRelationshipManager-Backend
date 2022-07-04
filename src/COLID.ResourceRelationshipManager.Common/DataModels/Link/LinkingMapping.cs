using COLID.ResourceRelationshipManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.DataModels.Link
{
    public class LinkingMapping
    {
        public LinkType LinkType { get; set; }
        public string PidUri { get; set; }
        public string InboundLinkLabel { get; set; }
        public string InboundLinkComment { get; set; }

        public LinkingMapping(LinkType linkType, string pidUri)
        {
            LinkType = linkType;
            PidUri = pidUri;
        }

        public void setLabel(string label)
        {
            this.InboundLinkLabel = label;
        }
        public void setComment(string comment)
        {
            this.InboundLinkComment = comment;
        }
    }
}
