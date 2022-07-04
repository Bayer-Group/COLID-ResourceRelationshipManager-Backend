using COLID.Graph.Metadata.DataModels.Metadata;
using COLID.Graph.TripleStore.DataModels.Base;
using COLID.Graph.TripleStore.Extensions;
using COLID.ResourceRelationshipManager.Common.DataModels;
using COLID.ResourceRelationshipManager.Repositories.Interface;
using COLID.ResourceRelationshipManager.Services.Interface;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using COLID.Exception.Models;
using COLID.ResourceRelationshipManager.Common.DataModels.Entity;

namespace COLID.ResourceRelationshipManager.Services.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class GraphMapService : IGraphMapService
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IGraphMapRepository _repo;
        private readonly IRemoteRegistrationService _remoteRegistrationService;
        private readonly ILogger<GraphMapService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="userInfoService"></param>
        /// <param name="remoteRegistrationService"></param>
        public GraphMapService(IGraphMapRepository repo, IUserInfoService userInfoService, IRemoteRegistrationService remoteRegistrationService,
            ILogger<GraphMapService> logger
            )
        {
            _userInfoService = userInfoService;
            _repo = repo;
            _remoteRegistrationService = remoteRegistrationService;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offest"></param>
        /// <returns></returns>
        public async Task<IList<GraphMap>> GetAllGraphMaps(int limit, int offest)
        {
            var graphMaps = await _repo.GetAllGraphMaps(limit, offest);
            return graphMaps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphMapSearchDTO"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<IList<GraphMap>> GetPageGraphMaps(GraphMapSearchDTO graphMapSearchDTO, int offset)
        {
            var graphMaps = await _repo.GetPageGraphMaps(graphMapSearchDTO, offset);
            return graphMaps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<IList<GraphMap>> GetGraphMapsForUser(string userId, int limit, int offset)
        {
            var graphMaps = await _repo.GetGraphMapsForUser(userId, limit, offset);
            return graphMaps;
        }

        /// <summary>
        /// return the maps containing particular resource
        /// </summary>
        /// <param name="pidUri"></param>
        /// <returns></returns>
        public async Task<IList<GraphMap>> GetGraphMapForResource(Uri pidUri)
        {
            var graphMaps = await _repo.GetGraphMapForResource(pidUri);
            return graphMaps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public async Task<GraphMap> GetGraphMapById(string mapId)
        {
            var graphMap = await _repo.GetGraphMapById(mapId);
            // compare graph
            await CompareSingleGraphMap(graphMap);
            return graphMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public async Task<GraphMap> GetGraphMapByName(string mapName)
        {
            var graphMap = await _repo.GetGraphMapByName(mapName);
            return graphMap;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphMap"></param>
        public async Task SaveGraphMap(GraphMap graphMap)
        {
            if (graphMap.GraphMapId == Guid.Empty && GetGraphMapByName(graphMap.Name) != null)
            {
                throw new BusinessException(message: $"Graph with the name '{graphMap.Name}' already exist!");
            }
            //in case of new graph
            if (graphMap.GraphMapId == null || graphMap.GraphMapId == Guid.Empty)
                graphMap.ModifiedBy = _userInfoService.GetEmail();

            graphMap.ModifiedAt = DateTime.UtcNow;
            await _repo.SaveGraphMap(graphMap);
        }

        /// <summary>
        /// Delete graph from database
        /// </summary>
        /// <param name="graphMap"></param>
        /// <returns></returns>
        public async Task DeleteGraphMap(GraphMap graphMap)
        {
            await _repo.DeleteGraphMap(graphMap);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceUris"></param>
        /// <returns></returns>
        public async Task<IList<ResourceDTO>> GetResourcesFromTripleStore(List<Uri> resourceUris)
        {
            List<ResourceDTO> result = new List<ResourceDTO>();
            var response = await _remoteRegistrationService.GetLinksAndResourcesForGraph(resourceUris);

            result = response.Select(graphResource => new ResourceDTO
            {
                ResourceIdentifier = graphResource.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.EnterpriseCore.PidUri, true).Id,
                ResourceType = graphResource.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.RDF.Type, true),
                Name = graphResource.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.Resource.HasLabel, true),
                PidUri = graphResource.PidUri,
                Links = mapLinkToMapLinkInfo(graphResource),
                Versions = graphResource.Versions,
                PreviousVersion = graphResource.PreviousVersion,
                LaterVersion = graphResource.LaterVersion
            }).ToList();

            return result;
        }

        private IList<MapLinkInfo> mapLinkToMapLinkInfo(ResourceCTO resource)
        {
            IList<MapLinkInfo> _formattedLinks = new List<MapLinkInfo>();
            if (resource.CustomLinks is List<dynamic>)
            {
                _formattedLinks = ((List<dynamic>)resource.CustomLinks).Select(x => new MapLinkInfo()
                {
                    StartNode = new NameValuePair() { Name = x.startNodeName.Value, Value = x.startNodeId.Value },
                    EndNode = new NameValuePair() { Name = x.endNodeName.Value, Value = x.endNodeId.Value },
                    Status = new Common.DataModels.Entity.Status(x.status.Value),
                    Type = new NameValuePair() { Name = x.type.name.Value, Value = x.type.value.Value }
                }).ToList();
            }
            return _formattedLinks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphs"></param>
        private async Task CompareGraphMaps(IList<GraphMap> graphs)
        {
            foreach (var graph in graphs)
            {
                await CompareSingleGraphMap(graph);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphMap"></param>
        private async Task CompareSingleGraphMap(GraphMap graphMap)
        {
            /*
             Need to refactor the below to adjust the status of the resource and links available in the Graph.
             Compare with the right property or introduce new property when needed.
             */
            var resourceUris = graphMap.MapNodes.Select(x => new Uri(x.ResourceIdentifier)).ToList();

            //fetch data from Registration service
            var response = await _remoteRegistrationService.GetLinksAndResourcesForGraph(resourceUris);

            var links = response.SelectMany(x => x.Links.SelectMany(x => x.Value)).ToList();

            var deletedResourceList = new List<MapNode>();
            var deletedLinkList = new List<MapLink>();

            //check for resource
            graphMap.MapNodes.ToList().ForEach(resource =>
            {
                if (!response.Any(x => x.PidUri == resource.PidUri))
                {
                    //delete the resource / node from resource relationship manager as well
                    deletedResourceList.Add(resource);
                    //delete links associated with the resource
                    var associatedLinks = links.Where(x => x.PidUri == resource.PidUri.AbsoluteUri).Select(x => x.PidUri);

                    //make a list for final deletion
                    deletedLinkList.AddRange(
                        graphMap.MapLinks.Where(x => associatedLinks.Contains(x.Source.AbsoluteUri) || associatedLinks.Contains(x.Target.AbsoluteUri))
                        );
                }
            });

            //remove deleted resource links from graph
            foreach (var deletedLink in deletedLinkList)
            {
                graphMap.MapLinks.Remove(deletedLink);
            }

            foreach (var deletedResource in deletedResourceList)
            {
                graphMap.MapNodes.Remove(deletedResource);
            }
        }

        public async Task<IList<LinkResourceTypeDTO>> GetLinkResourceTypes(string sourceUri, string targetUri)
        {
            try
            {

                var sourceResourceLinks = await GetResourceLinks(sourceUri);
                var targetResourceLinks = await GetResourceLinks(targetUri);

                var sourceResourceType = sourceResourceLinks.SelectMany(x => x.Properties["type"]).FirstOrDefault();
                var targetResourceType = targetResourceLinks.SelectMany(x => x.Properties["type"]).FirstOrDefault();

                List<LinkResourceTypeDTO> sourceLinkTypes = sourceResourceLinks.Where(
                 x => x.Properties["range"].FirstOrDefault().ToString().Contains(targetResourceType))
                .Select(x => new LinkResourceTypeDTO
                {
                    LinkType = new NameValuePair()
                    {
                        Value = x.Properties.GetValueOrNull("value", true),
                        Name = x.Properties.GetValueOrNull("name", true)
                    },
                    SourceUri = sourceUri,
                    TargetUri = targetUri,

                }).ToList();

                List<LinkResourceTypeDTO> targetLinkTypes = targetResourceLinks.Where(
                x => x.Properties["range"].FirstOrDefault().ToString().Contains(sourceResourceType))
                .Select(x => new LinkResourceTypeDTO
                {
                    LinkType = new NameValuePair()
                    {
                        Value = x.Properties.GetValueOrNull("value", true),
                        Name = x.Properties.GetValueOrNull("name", true)
                    },
                    SourceUri = targetUri,
                    TargetUri = sourceUri,
                }).ToList();

                return sourceLinkTypes.Concat(targetLinkTypes).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning("Error occured while fetching eligible links {sourceUri}, {targetUri}", sourceUri, targetUri, ex.Message);
                return new List<LinkResourceTypeDTO>();
            }
        }

        private async Task<List<Entity>> GetResourceLinks(string resourceUri)
        {
            var graphResource = await _remoteRegistrationService.GetResourcesFromGraph(new Uri(resourceUri));
            var resourceType = graphResource.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.RDF.Type, true);
            List<MetadataProperty> resourceMetadata = await _remoteRegistrationService.GetResourceTypes(new Uri(resourceType));


            var linksRange = resourceMetadata.Where(
             x => x.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.Shacl.Group, true)["key"].Value == COLID.Graph.Metadata.Constants.Resource.Groups.LinkTypes)
                            .Select(x => new
                            {
                                value = x.Properties.GetValueOrNull("http://pid.bayer.com/kos/19014/hasPID", true),

                                name = x.Properties.GetValueOrNull(Graph.Metadata.Constants.Shacl.Name, true),

                                range = x.Properties.GetValueOrNull("http://www.w3.org/2000/01/rdf-schema#range", true),

                                type = resourceType
                            }).ToList();
            List<Entity> linkEntities = new List<Entity>();

            foreach (var links in linksRange.Where(x => x.range != null))
            {
                var ety = new Entity();
                var linkProperties = new Dictionary<string, List<dynamic>>();
                linkProperties.Add("value", new List<dynamic> { links.value });
                linkProperties.Add("name", new List<dynamic> { links.name });
                linkProperties.Add("range", new List<dynamic> { links.range });
                linkProperties.Add("type", new List<dynamic> { links.type });

                ety.Properties = linkProperties;

                linkEntities.Add(ety);
            }

            var linksInstantiableTypes = await _remoteRegistrationService.GetInstantiableLinks(linkEntities);

            return linksInstantiableTypes;

        }
        public async Task<IList<Entity>> ManageResourceLinking(List<LinkResourceTypeDTO> linkResourceTypes)
        {
            List<Entity> result = new List<Entity>();
            foreach (var linkResourceType in linkResourceTypes)
            {
                //decide based on the action property
                switch (linkResourceType.Action)
                {
                    case Common.Enums.LinkAction.Add:
                        var linkResource = await _remoteRegistrationService.LinkResource(linkResourceType, _userInfoService.GetEmail());
                        result.Add(linkResource);
                        break;

                    case Common.Enums.LinkAction.Remove:
                        var unlinkResource = await _remoteRegistrationService.UnlinkResource(linkResourceType, _userInfoService.GetEmail());
                        result.Add(unlinkResource);
                        break;
                }
            }
            return result;
        }
    }
}
