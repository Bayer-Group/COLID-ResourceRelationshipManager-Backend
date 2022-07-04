using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using COLID.Graph.TripleStore.DataModels.Base;
using COLID.ResourceRelationshipManager.Common.DataModels;
using COLID.ResourceRelationshipManager.Common.DataModels.Entity;

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
        /// <returns></returns>
        Task<IList<GraphMap>> GetAllGraphMaps(int limit, int offest);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphMapSearchDTO"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<IList<GraphMap>> GetPageGraphMaps(GraphMapSearchDTO graphMapSearchDTO, int offset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<IList<GraphMap>> GetGraphMapsForUser(string userId, int limit, int offset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        Task<GraphMap> GetGraphMapById(string mapId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        Task<GraphMap> GetGraphMapByName(string mapName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphMap"></param>
        Task SaveGraphMap(GraphMap graphMap);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphMap"></param>
        /// <returns></returns>
        Task DeleteGraphMap(GraphMap graphMap);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceUris"></param>
        /// <returns></returns>
        Task<IList<ResourceDTO>> GetResourcesFromTripleStore(List<Uri> resourceUris);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceUri"></param>
        /// <param name="targetUri"></param>
        /// <returns></returns>
        Task<IList<LinkResourceTypeDTO>> GetLinkResourceTypes(string sourceUri, string targetUri);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkResourceTypes"></param>
        /// <returns></returns>
        Task<IList<Entity>> ManageResourceLinking(List<LinkResourceTypeDTO> linkResourceTypes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pidUri"></param>
        /// <returns></returns>
        Task<IList<GraphMap>> GetGraphMapForResource(Uri pidUri);
    }
}
