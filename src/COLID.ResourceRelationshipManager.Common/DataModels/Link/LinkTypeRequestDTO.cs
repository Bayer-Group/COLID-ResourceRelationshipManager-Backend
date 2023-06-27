using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.DataModels
{
    /// <summary>
    /// 
    /// </summary>
    public class LinkTypeRequestDTO
    {
        public Uri SourceUri { get; set; }
        public Uri TargetUri { get; set; }
    }
}
