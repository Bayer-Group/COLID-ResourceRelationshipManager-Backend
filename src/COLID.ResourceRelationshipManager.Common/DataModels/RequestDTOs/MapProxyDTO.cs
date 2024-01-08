using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLID.ResourceRelationshipManager.Common.DataModels.RequestDTOs
{
    public class MapProxyDTO
    {
        public Guid MapId { get; set; }
        public string PidUri { get; set; }

        public MapProxyDTO(Guid mapId)
        {
            MapId = mapId;
            PidUri = null;
        }

        public MapProxyDTO(Guid mapId, string pidUri)
        {
            MapId = mapId;
            PidUri = pidUri;
        }
        public MapProxyDTO()
        {

        }
    }
}
