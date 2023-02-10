using COLID.ResourceRelationshipManager.Common.DataModels.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COLID.ResourceRelationshipManager.Repositories.Interface
{
    public interface IGraphMapRepository
    {
        Task<IList<RelationMap>> GetAllGraphMaps(int limit, int offset);
        Task<IList<RelationMap>> GetPageGraphMaps(GraphMapSearchDTO graphMapSearchDTO, int offset);
        Task<IList<RelationMap>> GetGraphMapsForUser(string userId, int limit, int offset);
        Task<IList<RelationMap>> GetGraphMapForResource(Uri pidUri);
        Task<RelationMap> GetRelationMapByName(string relationName);
        Task<Guid> SaveRelationMap(RelationMap relationMap, bool isNew);
        Task<RelationMap> GetRelationMapById(string relationMapId);
        Task DeleteNodesByRelationMapId(Guid relationMapId);
        Task DeleteRelationMap(RelationMap relationMap);
    }
}
