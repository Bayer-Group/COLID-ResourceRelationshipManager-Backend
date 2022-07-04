using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.DataModels.Entity
{
    public class GraphMapSearchDTO
    {
        public int batchSize { get; set; } = 0;
        public string nameFilter { get; set; } = string.Empty;
        public string sortKey { get; set; } = string.Empty;
        public string sortType { get; set; } = string.Empty;
    }
}
