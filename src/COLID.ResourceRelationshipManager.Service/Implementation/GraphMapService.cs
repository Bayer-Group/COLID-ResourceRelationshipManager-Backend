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
using Newtonsoft.Json.Linq;
using COLID.ResourceRelationshipManager.Common.DataModels.NewTOs;
using System.Web;
using Newtonsoft.Json;
using COLID.ResourceRelationshipManager.Common.DataModels.RequestDTOs;
using COLID.ResourceRelationshipManager.Common.DataModels.Resource;
using System.Net.Http;
using COLID.Common.Extensions;

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
        private readonly IRemoteSearchService _remoteSearchService;
        private readonly ILogger<GraphMapService> _logger;

        private const string SCHEMA_RANGE = "http://www.w3.org/2000/01/rdf-schema#range";
        private const string TYPE = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="userInfoService"></param>
        /// <param name="remoteRegistrationService"></param>
        public GraphMapService(IGraphMapRepository repo, IUserInfoService userInfoService, IRemoteRegistrationService remoteRegistrationService, IRemoteSearchService remoteSearchService,
            ILogger<GraphMapService> logger
            )
        {
            _userInfoService = userInfoService;
            _repo = repo;
            _remoteRegistrationService = remoteRegistrationService;
            _remoteSearchService = remoteSearchService;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offest"></param>
        /// <returns></returns>
        public async Task<IList<RelationMapResponceDTO>> GetAllGraphMaps(int limit, int offest)
        {
            var relationMaps = await _repo.GetAllGraphMaps(limit, offest);
            List<RelationMapResponceDTO> relationMapResponceDTOs = new List<RelationMapResponceDTO>();
            foreach (var relationMap in relationMaps)
            {
                relationMapResponceDTOs.Add(new RelationMapResponceDTO()
                {
                    Id = relationMap.Id,
                    Description = relationMap.Description,
                    Name = relationMap.Name,
                    NodeCount = relationMap.NodeCount,
                    ModifiedBy = relationMap.ModifiedBy,
                    ModifiedAt = relationMap.ModifiedAt
                });
            }

            return relationMapResponceDTOs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphMapSearchDTO"></param>
        /// <param name="offset"></param>
        /// <returns>relationmap</returns>
        public async Task<IList<RelationMap>> GetPageGraphMaps(GraphMapSearchDTO graphMapSearchDTO, int offset)
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
        /// <returns>relationmap</returns>
        public async Task<IList<RelationMap>> GetGraphMapsForUser(string userId, int limit, int offset)
        {
            var relationMaps = await _repo.GetGraphMapsForUser(userId, limit, offset);
            return relationMaps;
        }

        /// <summary>
        /// return the maps containing particular resource
        /// </summary>
        /// <param name="pidUri"></param>
        /// <returns></returns>
        public async Task<IList<RelationMapResponceDTO>> GetGraphMapForResource(Uri pidUri)
        {
            var relationMaps = await _repo.GetGraphMapForResource(pidUri);
            List<RelationMapResponceDTO> relationMapResponceDTOs = new List<RelationMapResponceDTO>();
            foreach (var relationMap in relationMaps)
            {
                relationMapResponceDTOs.Add(new RelationMapResponceDTO()
                {
                    Id = relationMap.Id,
                    Description = relationMap.Description,
                    Name = relationMap.Name,
                    NodeCount = relationMap.NodeCount,
                    ModifiedBy = relationMap.ModifiedBy,
                    ModifiedAt = relationMap.ModifiedAt
                });
            }

            return relationMapResponceDTOs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceUris"></param>
        /// <returns></returns>
        public async Task<IList<ResourceDTO>> GetResourcesFromTripleStore(IList<Uri> resourceUris)
        {
            List<ResourceDTO> result = new List<ResourceDTO>();
            var response = await _remoteRegistrationService.GetLinksAndResourcesForGraph(resourceUris);

            result = response.Select(graphResource => new ResourceDTO
            {
                ResourceIdentifier = graphResource.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.EnterpriseCore.PidUri, true).Id,
                ResourceType = graphResource.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.RDF.Type, true),
                Name = graphResource.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.Resource.HasLabel, true),
                PidUri = graphResource.PidUri,
                Links = MapLinkToMapLinkInfo(graphResource),
                Versions = graphResource.Versions,
                PreviousVersion = graphResource.PreviousVersion,
                LaterVersion = graphResource.LaterVersion
            }).ToList();

            return result;
        }

        private static IList<MapLinkInfo> MapLinkToMapLinkInfo(ResourceCTO resource)
        {
            IList<MapLinkInfo> _formattedLinks = new List<MapLinkInfo>();
            if (resource.CustomLinks is List<dynamic> customLinks)
            {
                _formattedLinks = customLinks.Select(x => new MapLinkInfo()
                {
                    StartNode = new NameValuePair() { Name = x.startNodeName.Value, Value = x.startNodeId.Value, ResourceType = x.startNodeType.Value },
                    EndNode = new NameValuePair() { Name = x.endNodeName.Value, Value = x.endNodeId.Value, ResourceType = x.endNodeType.Value },
                    Status = new Status(x.status.Value),
                    Type = new NameValuePair() { Name = x.type.name.Value, Value = x.type.value.Value }
                }).ToList();
            }
            return _formattedLinks;
        }

        public async Task<IList<LinkResourceTypeDTO>> GetLinkResourceTypes(Uri sourceUri, Uri targetUri)
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
            catch (HttpRequestException ex)
            {
                _logger.LogWarning("Error occured while fetching eligible links {sourceUri}, {targetUri}, exception: {Message}", sourceUri, targetUri, ex.Message);
                return new List<LinkResourceTypeDTO>();
            }
        }

        private async Task<List<Entity>> GetResourceLinks(Uri resourceUri)
        {
            var graphResource = await _remoteRegistrationService.GetResourcesFromGraph(resourceUri);
            var resourceType = graphResource.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.RDF.Type, true);
            List<MetadataProperty> resourceMetadata = await _remoteRegistrationService.GetResourceTypes(new Uri(resourceType));

            var linksRange = resourceMetadata.Where(
             x => x.Properties.GetValueOrNull(COLID.Graph.Metadata.Constants.Shacl.Group, true)["key"].Value == COLID.Graph.Metadata.Constants.Resource.Groups.LinkTypes)
                            .Select(x => new
                            {
                                value = x.Properties.GetValueOrNull(Graph.Metadata.Constants.Resource.hasPID, true),

                                name = x.Properties.GetValueOrNull(Graph.Metadata.Constants.Shacl.Name, true),

                                range = x.Properties.GetValueOrNull(SCHEMA_RANGE, true),

                                type = resourceType
                            }).ToList();
            List<Entity> linkEntities = new List<Entity>();

            foreach (var links in linksRange.Where(x => x.range != null))
            {
                var ety = new Entity();
                var linkProperties = new Dictionary<string, List<dynamic>>
                {
                    { "value", new List<dynamic> { links.value } },
                    { "name", new List<dynamic> { links.name } },
                    { "range", new List<dynamic> { links.range } },
                    { "type", new List<dynamic> { links.type } }
                };

                ety.Properties = linkProperties;

                linkEntities.Add(ety);
            }

            var linksInstantiableTypes = await _remoteRegistrationService.GetInstantiableLinks(linkEntities);

            return linksInstantiableTypes;
        }

        public async Task<IList<Entity>> ManageResourceLinking(IList<LinkResourceTypeDTOV2> linkResourceTypes)
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

        /// <summary>
        /// Get Relation Map By Name
        /// </summary>
        /// <param name="relationMapName"></param>
        /// <returns></returns>
        public async Task<RelationMap> GetRelationMapByName(string relationMapName)
        {
            var relationMap = await _repo.GetRelationMapByName(relationMapName);
            return relationMap;
        }

        public async Task<RelationMap> GetPlainRelationMapById(string relationMapId)
        {
            return await _repo.GetRelationMapById(relationMapId);
        }

        /// <summary>
        /// Save Relation Map
        /// </summary>
        /// <param name="graphMapV2SaveDto"></param>
        public async Task<GraphMapTO> SaveRelationMap(GraphMapV2SaveDto graphMapV2SaveDto)
        {
            RelationMap relationMap;
            List<Node> nodes = new List<Node>();
            bool isNew = false;
            if (graphMapV2SaveDto.nodes.Count > 0)
            {
                foreach (var node in graphMapV2SaveDto.nodes)
                {
                    nodes.Add(new Node { PIDUri = new Uri(node.Id), xPosition = node.fx, yPosition = node.fy });
                }
            }

            if (string.IsNullOrEmpty(graphMapV2SaveDto.Id))
            {
                isNew = true;
                relationMap = new RelationMap() { Id = Guid.NewGuid(), Name = graphMapV2SaveDto.name, Description = graphMapV2SaveDto.description, Nodes = nodes };
            }
            else
            {
                relationMap = new RelationMap() { Id = Guid.Parse(graphMapV2SaveDto.Id), Name = graphMapV2SaveDto.name, Description = graphMapV2SaveDto.description, Nodes = nodes };
            }

            if (isNew && await GetRelationMapByName(relationMap.Name) != null)
            {
                throw new BusinessException(message: $"Graph with the name '{relationMap.Name}' already exist!");
            }

            relationMap.ModifiedBy = _userInfoService.GetEmail();
            relationMap.ModifiedAt = DateTime.UtcNow;
            Guid relationMapId = await _repo.SaveRelationMap(relationMap, isNew);

            return await GetRelationMapById(relationMapId.ToString());
        }

        /// <summary>
        /// Delete Relation Map from database
        /// </summary>
        /// <param name="relationMap"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRelationMap(RelationMap relationMap, bool isSuperAdmin = false)
        {
            string mapOwner = _userInfoService.GetEmail();

            if (!isSuperAdmin && relationMap.ModifiedBy != mapOwner)
            {
                return false;
            }

            // delete nodes before deletiong relation map
            await _repo.DeleteRelationMap(relationMap);
            return true;
        }

        /// <summary>
        /// Get RelationMap By Id
        /// </summary>
        /// <param name="relationMapId"></param>
        /// <returns></returns>
        public async Task<GraphMapTO> GetRelationMapById(string relationMapId)
        {
            GraphMapTO resultGraphMap = new GraphMapTO();
            List<MapNodeTO> mapNodeTOList = new List<MapNodeTO>();
            Dictionary<string, string> allLinks = new Dictionary<string, string>();
            IDictionary<string, IEnumerable<JObject>> response = new Dictionary<string, IEnumerable<JObject>>();
            var relationMap = await _repo.GetRelationMapById(relationMapId);

            relationMap.Nodes = relationMap.Nodes.GroupBy(x => x.PIDUri).Select(x => x.First()).ToList();
            try
            {
                response = await _remoteSearchService.GetDocumentsByIds(relationMap.Nodes.Select(x => x.PIDUri.ToString()));
                _logger.LogInformation("Successfully fetched documents from search service. Count: {Count}", response.Count);
                allLinks = await _remoteRegistrationService.GetLinkTypes();
                _logger.LogInformation("Successfully fetched all link types");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Response from search service GetDocumentsByIds: Exception StackTrace: {StackTrace}, Exception Message: {Message}", ex.StackTrace, ex.Message);
                throw;
            }
            if (response.Count > 0)
            {
                try
                {
                    _logger.LogInformation("Processing response {Response}", JsonConvert.SerializeObject(response));
                    foreach (var node in relationMap.Nodes)
                    {
                        string encodedPidUri = HttpUtility.UrlEncode(node.PIDUri.ToString());
                        response.TryGetValue(encodedPidUri, out var pidUriData);

                        _logger.LogInformation("Got PID URI Data from response {Response}", JsonConvert.SerializeObject(pidUriData));
                        foreach (var data in pidUriData)
                        {
                            if (data != null)
                            {
                                var resourceName = data[Graph.Metadata.Constants.Resource.HasLabel]["outbound"][0]["value"].ToString();
                                var resourceTypeUri = data[TYPE]["outbound"][0]["uri"].ToString();
                                var mapLinksAndTargetDTO = GetMapLinks(data, node.PIDUri.ToString(), resourceName, resourceTypeUri, allLinks);
                                var hasLaterVersion = data[Graph.Metadata.Constants.Resource.HasLaterVersion] == null ? "" : data[Graph.Metadata.Constants.Resource.HasLaterVersion]["outbound"][0]["value"].ToString();

                                if (!hasLaterVersion.IsNullOrEmpty() && mapLinksAndTargetDTO.TargetURIs.Count > 0)
                                {
                                    List<TargetDTO> targetNameAndTypeList = GetTargetNameAndTypeFromSearchService(mapLinksAndTargetDTO.TargetURIs).Result;

                                    foreach (var mapLink in mapLinksAndTargetDTO.MapLinks)
                                    {
                                        foreach (var target in targetNameAndTypeList)
                                        {
                                            if (target.TargetPIDUri.ToString() == mapLink.Target)
                                            {
                                                mapLink.TargetName = target.TargetName;
                                                mapLink.TargetType = target.TargetType;
                                            }
                                        }
                                    }
                                }

                                mapNodeTOList.Add(new MapNodeTO
                                {
                                    Id = node.PIDUri.ToString(),
                                    Fx = node.xPosition,
                                    Fy = node.yPosition,
                                    Name = resourceName,
                                    LaterVersion = hasLaterVersion,
                                    ShortName = GenerateShortName(data[Graph.Metadata.Constants.Resource.HasLabel]["outbound"][0]["value"].ToString()),

                                    ResourceType = new KeyValuePair<string, string>(
                                            resourceTypeUri,
                                            data[TYPE]["outbound"][0]["value"].ToString()
                                            ),

                                    Links = mapLinksAndTargetDTO.MapLinks
                                });
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError("Error occured while converting response to model with error message: {ErrorMessage}, stacktrace: {StackTrace}, response {Response}", ex.Message, ex.StackTrace, JsonConvert.SerializeObject(response));
                    throw;
                }
            }

            resultGraphMap = new GraphMapTO() { Id = relationMap.Id, Name = relationMap.Name, Description = relationMap.Description, ModifiedAt = relationMap.ModifiedAt, ModifiedBy = relationMap.ModifiedBy, Nodes = mapNodeTOList };

            _logger.LogInformation("Returning processed result {Result}", JsonConvert.SerializeObject(mapNodeTOList));
            return resultGraphMap;
        }

        /// <summary>
        /// Fetch resources from the serach service and return a plain list
        /// </summary>
        /// <param name="uris">IDs of the resources</param>
        /// <returns>List of Map Node objects</returns>
        public async Task<List<MapNodeTO>> GetResources(IList<Uri> uris)
        {
            //Convert URI list to string list
            List<string> identifiers = new List<string>();
            List<Uri> targetUris = new List<Uri>();
            Dictionary<string, string> allLinks = new Dictionary<string, string>();

            foreach (Uri uri in uris)
            {
                identifiers.Add(uri.ToString());
            }

            //Fetch results from search service
            List<MapNodeTO> resultMapNodes = new List<MapNodeTO>();
            IDictionary<string, IEnumerable<JObject>> response = new Dictionary<string, IEnumerable<JObject>>();
            try
            {
                response = await _remoteSearchService.GetDocumentsByIds(identifiers);
                allLinks = await _remoteRegistrationService.GetLinkTypes();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Response from search service GetDocumentsByIds failed with message: {Message}", ex.Message);
                throw;
            }

            //Process results into object list
            if (response.Count > 0)
            {
                foreach (var pidUri in identifiers)
                {
                    string encodedPidUri = HttpUtility.UrlEncode(pidUri);
                    var resourceData = response[encodedPidUri];

                    foreach (var data in resourceData)
                    {
                        if (data != null)
                        {
                            var resourceName = data[Graph.Metadata.Constants.Resource.HasLabel]["outbound"][0]["value"].ToString();
                            var resourceTypeUri = data[TYPE]["outbound"][0]["uri"].ToString();
                            var maplinks = GetMapLinks(data, pidUri, resourceName, resourceTypeUri, allLinks);
                            var hasLaterVersion = data[Graph.Metadata.Constants.Resource.HasLaterVersion] == null ? "" : data[Graph.Metadata.Constants.Resource.HasLaterVersion]["outbound"][0]["value"].ToString();
                            targetUris.AddRange(maplinks.TargetURIs);

                            if (targetUris.Count > 0)
                            {
                                List<TargetDTO> targetNameAndTypeList = GetTargetNameAndTypeFromSearchService(targetUris).Result;

                                foreach (var mapLink in maplinks.MapLinks)
                                {
                                    foreach (var target in targetNameAndTypeList)
                                    {
                                        if (target.TargetPIDUri.ToString() == mapLink.Target)
                                        {
                                            mapLink.TargetName = target.TargetName;
                                            mapLink.TargetType = target.TargetType;
                                        }
                                    }
                                }
                            }

                            resultMapNodes.Add(new MapNodeTO
                            {
                                Id = pidUri.ToString(),
                                Fx = 0,
                                Fy = 0,
                                Name = resourceName,
                                ShortName = GenerateShortName(resourceName),
                                LaterVersion = hasLaterVersion,
                                ResourceType = new KeyValuePair<string, string>(
                                    resourceTypeUri,
                                    data[TYPE]["outbound"][0]["value"].ToString()
                                ),
                                Links = maplinks.MapLinks
                            });
                        }
                    }
                }
            }

            return resultMapNodes;
        }

        /// <summary>
        /// Fill MapLinkTO from PIDURI metadata responce 
        /// </summary>
        /// <param name="linksData"></param>
        /// <param name="sourcePIDUri"></param>
        /// <returns></returns>
        private static MapLinksAndTargetDTO GetMapLinks(JObject linksData, string sourcePIDUri, string sourceName, string sourceType, Dictionary<string, string> allLinks)
        {
            List<Uri> targetURIs = new List<Uri>();
            List<MapLinkTO> mapLinks = new List<MapLinkTO>();

            //Looping LinkTypes for outbound data from PIDURI responce
            if (linksData[Graph.Metadata.Constants.Resource.Groups.LinkTypes] != null && linksData[Graph.Metadata.Constants.Resource.Groups.LinkTypes]["outbound"] != null)
                foreach (var outboundData in linksData[Graph.Metadata.Constants.Resource.Groups.LinkTypes]["outbound"])
                {
                    mapLinks.Add(CreateMapLinkTO(outboundData, sourcePIDUri, sourceName, sourceType, allLinks, true, false));
                }

            //Looping LinkTypes for inbound data from PIDURI responce
            if (linksData[Graph.Metadata.Constants.Resource.Groups.LinkTypes] != null && linksData[Graph.Metadata.Constants.Resource.Groups.LinkTypes]["inbound"] != null)
                foreach (var inboundData in linksData[Graph.Metadata.Constants.Resource.Groups.LinkTypes]["inbound"])
                {
                    mapLinks.Add(CreateMapLinkTO(inboundData, sourcePIDUri, sourceName, sourceType, allLinks, false, false));
                }

            //Looping Version for outound data from PIDURI responce
            if (linksData[Graph.Metadata.Constants.Resource.HasVersions] != null && linksData[Graph.Metadata.Constants.Resource.HasVersions]["outbound"].Any())
            {
                foreach (var outboundData in linksData[Graph.Metadata.Constants.Resource.HasVersions]["outbound"])
                {
                    string outboundTarget = outboundData["value"][Graph.Metadata.Constants.Resource.hasPID] != null ? outboundData["value"][Graph.Metadata.Constants.Resource.hasPID]["uri"].ToString() : "";
                    targetURIs.Add(new Uri(outboundTarget));

                    mapLinks.Add(CreateMapLinkTO(outboundData, sourcePIDUri, sourceName, sourceType, allLinks, true, true));
                }
            }

            //Looping Version for inbound data from PIDURI responce
            if (linksData[Graph.Metadata.Constants.Resource.HasVersions] != null && linksData[Graph.Metadata.Constants.Resource.HasVersions]["inbound"].Any())
            {
                foreach (var inboundData in linksData[Graph.Metadata.Constants.Resource.HasVersions]["inbound"])
                {
                    var inboundTarget = inboundData["value"][Graph.Metadata.Constants.Resource.hasPID] != null ? inboundData["value"][Graph.Metadata.Constants.Resource.hasPID]["uri"].ToString() : "";
                    targetURIs.Add(new Uri(inboundTarget));

                    mapLinks.Add(CreateMapLinkTO(inboundData, sourcePIDUri, sourceName, sourceType, allLinks, false, true));
                }
            }

            MapLinksAndTargetDTO mapLinksAndTargetDTO = new MapLinksAndTargetDTO() { MapLinks = mapLinks, TargetURIs = targetURIs };

            return mapLinksAndTargetDTO;
        }

        private static MapLinkTO CreateMapLinkTO(dynamic data, string sourcePidUri, string sourceName, string sourceType, Dictionary<string, string> allLinks, bool isOutbound, bool isVersionLink)
        {
            string linkType = data["edge"]?.ToString() ?? "";

            var mapLinkTO = new MapLinkTO()
            {
                IsVersionLink = isVersionLink,
                Outbound = isOutbound,
                Source = sourcePidUri,
                SourceName = sourceName,
                SourceType = sourceType,
                Target = isVersionLink
                    ? data["value"][Graph.Metadata.Constants.Resource.hasPID]?["uri"]?.ToString() ?? ""
                    : data["value"][Graph.Metadata.Constants.Resource.hasPID]?["outbound"][0]["uri"]?.ToString() ?? "",
                TargetName = isVersionLink
                    ? ""
                    : data["value"][Graph.Metadata.Constants.Resource.HasLabel]?["outbound"][0]["value"]?.ToString() ?? "",
                TargetType = isVersionLink
                    ? ""
                    : data["value"][Graph.Metadata.Constants.RDF.Type]?["outbound"][0]["uri"]?.ToString() ?? "",
                LinkType = isVersionLink
                    ? new KeyValuePair<string, string>
                        (
                            Graph.Metadata.Constants.Resource.HasVersion,
                            data["value"][Graph.Metadata.Constants.Resource.HasVersion]?["value"]?.ToString() ?? ""
                        )
                    : new KeyValuePair<string, string>
                        (
                            linkType,
                            allLinks.GetValueOrDefault(linkType)
                        )

            };

            return mapLinkTO;
        }

        /// <summary>
        /// Get Target Name and Type From Search Service
        /// </summary>
        /// <param name="TargetPIDUris"></param>
        /// <returns></returns>
        public async Task<List<TargetDTO>> GetTargetNameAndTypeFromSearchService(IList<Uri> TargetPIDUris)
        {
            List<TargetDTO> responceTarget = new List<TargetDTO>();
            var response = await _remoteSearchService.GetDocumentsByIds(TargetPIDUris.Select(uri => uri.ToString()));

            if (response.Count > 0)
            {
                foreach (var pidUri in TargetPIDUris)
                {
                    string encodedPidUri = HttpUtility.UrlEncode(pidUri.ToString());
                    var resourceData = response[encodedPidUri];

                    foreach (var data in resourceData)
                    {
                        if (data != null)
                        {
                            responceTarget.Add(new TargetDTO()
                            {
                                TargetPIDUri = pidUri,
                                TargetName = data[Graph.Metadata.Constants.Resource.HasLabel]["outbound"][0]["value"].ToString(),
                                TargetType = data[TYPE]["outbound"][0]["uri"].ToString()
                            });
                        }
                    }
                }
            }

            return responceTarget;
        }

        /// <summary>
        /// Generate short name from longname
        /// </summary>
        /// <param name="longName"></param>
        /// <returns></returns>
        public static string GenerateShortName(string longName)
        {
            string[] segments = longName.Split(" ");
            for (var i = 0; i < segments.Length; i++)
            {
                if (segments[i].Length > 3)
                {
                    segments[i] = segments[i][..3];
                }
            }
            string shortName = string.Join("-", segments);
            return shortName;
        }
    }
}
