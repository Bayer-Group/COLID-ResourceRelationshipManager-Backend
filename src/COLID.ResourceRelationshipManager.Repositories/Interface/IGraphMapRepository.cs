using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using COLID.ResourceRelationshipManager.Common.DataModels;
using COLID.ResourceRelationshipManager.Common.DataModels.Entity;

namespace COLID.ResourceRelationshipManager.Repositories.Interface
{
    public interface IGraphMapRepository
    {
        Task<IList<GraphMap>> GetAllGraphMaps(int limit, int offset);
        Task<IList<GraphMap>> GetPageGraphMaps(GraphMapSearchDTO graphMapSearchDTO, int offset);
        Task<IList<GraphMap>> GetGraphMapsForUser(string userId, int limit, int offset);
        Task<GraphMap> GetGraphMapById(string mapId);
        Task SaveGraphMap(GraphMap graphMap);
        Task DeleteGraphMap(GraphMap graphMap);
        Task<IList<GraphMap>> GetGraphMapForResource(Uri pidUri);
        Task<GraphMap> GetGraphMapByName(string mapName);
    }
}
