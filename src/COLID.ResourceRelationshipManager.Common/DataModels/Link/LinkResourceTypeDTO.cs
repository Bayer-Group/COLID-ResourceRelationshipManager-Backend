using COLID.ResourceRelationshipManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    /// <summary>
    /// RRM api accepts this type to fetch the link related information
    /// </summary>
    public class LinkResourceTypeDTO : LinkTypeRequestDTO
    {
        public NameValuePair LinkType { get; set; }
        public string Action { get; set; }
    }
}
