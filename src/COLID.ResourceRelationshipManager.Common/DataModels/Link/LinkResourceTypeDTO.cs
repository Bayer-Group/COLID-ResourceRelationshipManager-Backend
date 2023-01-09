using COLID.ResourceRelationshipManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    ///TODO: switch to new data model everywhere
    /// <summary>
    /// RRM api accepts this type to fetch the link related information
    /// </summary>
    public class LinkResourceTypeDTO : LinkTypeRequestDTO
    {
        public NameValuePair LinkType { get; set; }
        public string Action { get; set; }
    }

    /// <summary>
    /// RRM api accepts this type to fetch the link related information
    /// </summary>
    public class LinkResourceTypeDTOV2 : LinkTypeRequestDTO
    {
        public KeyValuePair<string, string> LinkType { get; set; }
        public string Action { get; set; }
    }
}
