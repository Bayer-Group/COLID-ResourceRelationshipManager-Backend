using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    public class LinkDTO
    {
        public NameValuePair Type { get; set; }
        public string Status { get; set; }
        public NameValuePair StartNode { get; set; }
        public NameValuePair EndNode { get; set; }
    }
    
}
