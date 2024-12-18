﻿using COLID.Graph.TripleStore.DataModels.Base;
using COLID.ResourceRelationshipManager.Common.DataModels;
using COLID.ResourceRelationshipManager.Common.DataModels.Entity;
using COLID.ResourceRelationshipManager.Common.DataModels.NewTOs;
using COLID.ResourceRelationshipManager.Common.DataModels.RequestDTOs;
using COLID.ResourceRelationshipManager.Common.DataModels.Resource;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COLID.ResourceRelationshipManager.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGraphMapService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offest"></param>
        /// <returns>RelationMap</returns>
        Task<IList<RelationMapResponseDTO>> GetAllGraphMaps(int limit, int offest);
        Task<IList<MapProxyDTO>> GetAllMapProxyDTOs();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphMapSearchDTO"></param>
        /// <param name="offset"></param>
        /// <returns>relationmap</returns>
        Task<IList<RelationMap>> GetPageGraphMaps(GraphMapSearchDTO graphMapSearchDTO, int offset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns>relationmap</returns>
        Task<IList<RelationMap>> GetGraphMapsForUser(string userId, int limit, int offset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceUris"></param>
        /// <returns></returns>
        Task<IList<ResourceDTO>> GetResourcesFromTripleStore(IList<Uri> resourceUris);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceUri"></param>
        /// <param name="targetUri"></param>
        /// <returns></returns>
        Task<IList<LinkResourceTypeDTO>> GetLinkResourceTypes(Uri sourceUri, Uri targetUri);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkResourceTypes"></param>
        /// <returns></returns>
        Task<IList<Entity>> ManageResourceLinking(IList<LinkResourceTypeDTOV2> linkResourceTypes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pidUri"></param>
        /// <returns></returns>
        Task<IList<RelationMapResponseDTO>> GetGraphMapForResource(Uri pidUri);

        /// <summary>
        /// Get Relation Map By Name
        /// </summary>
        /// <param name="relationMapName"></param>
        /// <returns></returns>
        Task<RelationMap> GetRelationMapByName(string relationMapName);

        /// <summary>
        /// Get Plain Relation Map By Id
        /// </summary>
        /// <param name="relationMapId"></param>
        /// <returns></returns>
        Task<RelationMap> GetPlainRelationMapById(string relationMapId);

        /// <summary>
        /// Save Relation Map
        /// </summary>
        /// <param name="graphMapV2SaveDto"></param>
        Task<GraphMapTO> SaveRelationMap(GraphMapV2SaveDto graphMapV2SaveDto);

        /// <summary>
        /// Delete Relation Map
        /// </summary>
        /// <param name="relationMap"></param>
        /// <returns></returns>
        Task<bool> DeleteRelationMap(RelationMap relationMap, bool isSuperAdmin = false);

        /// <summary>
        /// Get Relation Map By Id
        /// </summary>
        /// <param name="relationMapId"></param>
        /// <returns></returns>
        Task<GraphMapTO> GetRelationMapById(string relationMapId);

        /// <summary>
        /// Fetch resources from the search service and return a plain list
        /// </summary>
        /// <param name="uris">List of URIs</param>
        /// <returns>List of Map Node objects</returns>
        Task<List<MapNodeTO>> GetResources(IList<Uri> uris);

        Task<List<MapNodeTO>> GetFilteredResources(LinkResourcesFilterDTO resourceFilterRequest);

    }
}
