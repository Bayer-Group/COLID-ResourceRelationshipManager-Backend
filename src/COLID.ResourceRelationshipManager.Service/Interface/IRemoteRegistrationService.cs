using COLID.Graph.Metadata.DataModels.Metadata;
using COLID.Graph.TripleStore.DataModels.Base;
using COLID.ResourceRelationshipManager.Common.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COLID.ResourceRelationshipManager.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRemoteRegistrationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        Task<List<ResourceCTO>> GetLinksAndResourcesForGraph(List<Uri> resources);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourcesUris"></param>
        /// <returns></returns>
        Task<ResourceCTO> GetResourcesFromGraph(Uri resourcesUri);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceUris"></param>
        Task<List<MetadataProperty>> GetResourceTypes(Uri resourceUris);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkResourceDTO"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        Task<Entity> LinkResource(LinkResourceTypeDTO linkResourceDTO, string requester);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unlinkResourceDTO"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        Task<Entity> UnlinkResource(LinkResourceTypeDTO unlinkResourceDTO, string requester);
        /// 
        /// </summary>
        /// <param name="linksInstantiableTypes"></param>
        /// <returns></returns>
        Task<List<Entity>> GetInstantiableLinks(List<Entity> linksInstantiableTypes);
        /// <summary>
    }
}
