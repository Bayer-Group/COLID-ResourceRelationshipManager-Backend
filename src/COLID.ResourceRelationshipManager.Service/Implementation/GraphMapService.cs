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
using System.IO;
using Microsoft.Extensions.Configuration;

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
        private static readonly string _basePath = Path.GetFullPath("appsettings.json");
        private static readonly string _filePath = _basePath.Substring(0, _basePath.Length - 16);
        private static IConfigurationRoot configuration = new ConfigurationBuilder()
                     .SetBasePath(_filePath)
                    .AddJsonFile("appsettings.json")
                    .Build();
        private static readonly string _serviceUrl = configuration.GetValue<string>("ServiceUrl");
        private static readonly string _httpServiceUrl = configuration.GetValue<string>("HttpServiceUrl");



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
                    StartNode = new NameValuePair() { Name = x.startNodeName.Value, Value = x.startNodeId.Value, ResourceType = x.startNodeType.Value },
                    EndNode = new NameValuePair() { Name = x.endNodeName.Value, Value = x.endNodeId.Value, ResourceType = x.endNodeType.Value },
                    Status = new Common.DataModels.Entity.Status(x.status.Value),
                    Type = new NameValuePair() { Name = x.type.name.Value, Value = x.type.value.Value }
                }).ToList();
            }
            return _formattedLinks;
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
                                value = x.Properties.GetValueOrNull(_httpServiceUrl + "kos/19014/hasPID", true),

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

        public async Task<IList<Entity>> ManageResourceLinking(List<LinkResourceTypeDTOV2> linkResourceTypes)
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
            List<Nodes> nodes = new List<Nodes>();
            bool isNew = false;
            if (graphMapV2SaveDto.nodes.Count() > 0)
            {
                foreach (var node in graphMapV2SaveDto.nodes)
                {
                    nodes.Add(new Nodes { PIDUri = new Uri(node.Id), xPosition = node.fx, yPosition = node.fy });
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
            List<string> targetUris = new List<string>();
            Dictionary<string, string> allLinks = new Dictionary<string, string>();
            IDictionary<string, IEnumerable<JObject>> response = new Dictionary<string, IEnumerable<JObject>>();
            var relationMap = await _repo.GetRelationMapById(relationMapId);
            try
            {
                relationMap.Nodes = relationMap.Nodes.GroupBy(x => x.PIDUri).Select(x => x.First()).ToList();
                try
                {
                    response = await _remoteSearchService.GetDocumentsByIds(relationMap.Nodes.Select(x => x.PIDUri.ToString()));
                    _logger.LogInformation("Successfully fetched documents from search service. Count: " + response.Count);
                    allLinks = await _remoteRegistrationService.GetLinkTypes();
                    _logger.LogInformation("Successfully fetched all link types");
                }
                catch (System.Exception ex)
                {
                    _logger.LogError("Responce from search service GetDocumentsByIds: + Exception StackTrace: " + ex.StackTrace, "Exception Message: " + ex.Message);
                    _logger.LogError(JsonConvert.SerializeObject(ex));
                }
                if (response.Count > 0)
                {
                    try
                    {
                        _logger.LogInformation("Processing response, " + JsonConvert.SerializeObject(response));
                        foreach (var node in relationMap.Nodes)
                        {
                            string EncodedPIDUri = HttpUtility.UrlEncode(node.PIDUri.ToString());
                            var PIDUriData = response[EncodedPIDUri];

                            _logger.LogInformation("Got PID URI Data from response, " + JsonConvert.SerializeObject(PIDUriData));
                            foreach (var data in PIDUriData)
                            {
                                if (data != null)
                                {
                                    var resourceName = data[_serviceUrl + "kos/19050/hasLabel"]["outbound"][0]["value"].ToString();
                                    var resourceTypeUri = data["http://www.w3.org/1999/02/22-rdf-syntax-ns#type"]["outbound"][0]["uri"].ToString();
                                    var mapLinksAndTargetDTO = GetMapLinks(data, node.PIDUri.ToString(), resourceName, resourceTypeUri, allLinks);
                                    
                                    if(mapLinksAndTargetDTO.TargetURIs.Count() > 0)
                                    {
                                        //TODO: This has not any effect. move everything AFTER the outer foreach loop
                                        targetUris.AddRange(mapLinksAndTargetDTO.TargetURIs);

                                        List<TargetDTO> targetNameAndTypeList = GetTargetNameAndTypeFromSearchService(mapLinksAndTargetDTO.TargetURIs).Result;

                                        foreach (var mapLink in mapLinksAndTargetDTO.MapLinks)
                                        {
                                            foreach (var target in targetNameAndTypeList)
                                            {
                                                if (target.TargetPIDUri == mapLink.Target)
                                                {
                                                    mapLink.TargetName = target.TargetName;
                                                    mapLink.TargetType = target.TargetType;
                                                }
                                            }
                                        }
                                        //until here
                                    }

                                    _logger.LogInformation("Further processing data");
                                    mapNodeTOList.Add(new MapNodeTO
                                    {
                                        Id = node.PIDUri.ToString(),
                                        Fx = node.xPosition,
                                        Fy = node.yPosition,
                                        Name = resourceName,
                                        ShortName = GenerateShortName(data[_serviceUrl + "kos/19050/hasLabel"]["outbound"][0]["value"].ToString()),

                                        ResourceType = new KeyValuePair<string, string>(
                                                resourceTypeUri,
                                                data["http://www.w3.org/1999/02/22-rdf-syntax-ns#type"]["outbound"][0]["value"].ToString()
                                                ),

                                        Links = mapLinksAndTargetDTO.MapLinks
                                    });
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        _logger.LogError("Error occured while converting responce to model", "Error Message:" + ex.Message, "Stack Trace: " + ex.StackTrace, "Search Service Responce from GetDocumentsByIds" + response);
                    }
                }

                resultGraphMap = new GraphMapTO() { Id = relationMap.Id, Name = relationMap.Name, Description = relationMap.Description, ModifiedAt = relationMap.ModifiedAt, ModifiedBy = relationMap.ModifiedBy, Nodes = mapNodeTOList };
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Exception StackTrace: " + ex.StackTrace, "Exception Message: " + ex.Message);
            }
            _logger.LogInformation("Returning processed result " + JsonConvert.SerializeObject(mapNodeTOList));
            return resultGraphMap;
        }

        /// <summary>
        /// Fetch resources from the serach service and return a plain list
        /// </summary>
        /// <param name="uris">IDs of the resources</param>
        /// <returns>List of Map Node objects</returns>
        public async Task<List<MapNodeTO>> GetResources(List<Uri> uris)
        {
            //Convert URI list to string list
            List<string> identifiers = new List<string>();
            List<string> targetUris = new List<string>();
            Dictionary<string, string> allLinks = new Dictionary<string, string>();

            uris.ForEach(uri =>
            {
                identifiers.Add(uri.ToString());
            });

            //Fetch results from search service
            List<MapNodeTO> resultMapNodes = new List<MapNodeTO>();
            IDictionary<string, IEnumerable<JObject>> response = new Dictionary<string, IEnumerable<JObject>>();
            try
            {
                response = await _remoteSearchService.GetDocumentsByIds(identifiers);
                allLinks = await _remoteRegistrationService.GetLinkTypes();
            } 
            catch (System.Exception ex)
            {
                _logger.LogError("Response from search service GetDocumentsByIds failed.");
                _logger.LogError(JsonConvert.SerializeObject(ex));
            }

            //Process results into object list
            if(response.Count > 0)
            {
                foreach(var pidUri in identifiers)
                {
                    string encodedPidUri = HttpUtility.UrlEncode(pidUri);
                    var resourceData = response[encodedPidUri];

                    foreach(var data in resourceData)
                    {
                        if(data != null)
                        {
                            var resourceName = data[_serviceUrl + "kos/19050/hasLabel"]["outbound"][0]["value"].ToString();
                            var resourceTypeUri = data["http://www.w3.org/1999/02/22-rdf-syntax-ns#type"]["outbound"][0]["uri"].ToString();
                            var maplinks = GetMapLinks(data, pidUri, resourceName, resourceTypeUri, allLinks);
                            var hasLaterVersion = data["https://pid.bayer.com/kos/19050/hasLaterVersion"] == null? "" : data["https://pid.bayer.com/kos/19050/hasLaterVersion"]["outbound"][0]["value"].ToString();
                            targetUris.AddRange(maplinks.TargetURIs);

                            if(targetUris.Count > 0)
                            {
                                List<TargetDTO> targetNameAndTypeList = GetTargetNameAndTypeFromSearchService(targetUris).Result;

                                foreach (var mapLink in maplinks.MapLinks)
                                {
                                    foreach (var target in targetNameAndTypeList)
                                    {
                                        if (target.TargetPIDUri == mapLink.Target)
                                        {
                                            mapLink.TargetName = target.TargetName;
                                            mapLink.TargetType = target.TargetType;
                                        }
                                    }
                                }
                            }                            

                            resultMapNodes.Add(new MapNodeTO
                            {
                                Id = pidUri,
                                Fx = 0,
                                Fy = 0,
                                Name = resourceName,
                                ShortName = GenerateShortName(resourceName),
                                LaterVersion = hasLaterVersion,
                                ResourceType = new KeyValuePair<string, string>(
                                    resourceTypeUri,
                                    data["http://www.w3.org/1999/02/22-rdf-syntax-ns#type"]["outbound"][0]["value"].ToString()
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
        private MapLinksAndTargetDTO GetMapLinks(JToken linksData, string sourcePIDUri, string sourceName, string sourceType, Dictionary<string, string> allLinks)
        {
            List<string> targetURIs = new List<string>();
            List<MapLinkTO> mapLinks = new List<MapLinkTO>();
            try
            {
                //Looping LinkTypes for outbound data from PIDURI responce
                if (linksData[_httpServiceUrl + "kos/19050/LinkTypes"] != null && linksData[_httpServiceUrl + "kos/19050/LinkTypes"]["outbound"] != null)
                    foreach (var outboundData in linksData[_httpServiceUrl + "kos/19050/LinkTypes"]["outbound"])
                    {
                        var linkType = outboundData["edge"] != null ? outboundData["edge"].ToString() : "";
                        mapLinks.Add(new MapLinkTO()
                        {
                            IsVersionLink = false,
                            Outbound = true,
                            Source = sourcePIDUri,
                            SourceName = sourceName,
                            SourceType = sourceType,
                            Target = outboundData["value"][_httpServiceUrl + "kos/19014/hasPID"]["outbound"][0] != null ? outboundData["value"][_httpServiceUrl + "kos/19014/hasPID"]["outbound"][0]["uri"].ToString() : "",
                            TargetName = outboundData["value"][_serviceUrl + "kos/19050/hasLabel"]["outbound"][0] != null ? outboundData["value"][_serviceUrl + "kos/19050/hasLabel"]["outbound"][0]["value"].ToString() : "",
                            TargetType = outboundData["value"]["http://www.w3.org/1999/02/22-rdf-syntax-ns#type"]["outbound"][0] != null ? outboundData["value"]["http://www.w3.org/1999/02/22-rdf-syntax-ns#type"]["outbound"][0]["uri"].ToString() : "",
                            LinkType = new KeyValuePair<string, string>
                            (
                                outboundData["edge"] != null ? outboundData["edge"].ToString() : "",
                                allLinks.GetValueOrDefault(linkType)
                            )
                        });
                    }

                //Looping LinkTypes for inbound data from PIDURI responce
                if (linksData[_httpServiceUrl + "kos/19050/LinkTypes"] != null && linksData[_httpServiceUrl + "kos/19050/LinkTypes"]["inbound"] != null)
                    foreach (var inboundData in linksData[_httpServiceUrl + "kos/19050/LinkTypes"]["inbound"])
                    {
                        var linkType = inboundData["edge"] != null ? inboundData["edge"].ToString() : "";
                        mapLinks.Add(new MapLinkTO()
                        {
                            IsVersionLink = false,
                            Outbound = false,
                            Source = sourcePIDUri,
                            SourceName = sourceName,
                            SourceType = sourceType,
                            Target = inboundData["value"][_httpServiceUrl + "kos/19014/hasPID"]["outbound"][0] != null ? inboundData["value"][_httpServiceUrl + "kos/19014/hasPID"]["outbound"][0]["uri"].ToString() : "",
                            TargetName = inboundData["value"][_serviceUrl + "kos/19050/hasLabel"]["outbound"][0] != null ? inboundData["value"][_serviceUrl + "kos/19050/hasLabel"]["outbound"][0]["value"].ToString() : "",
                            TargetType = inboundData["value"]["http://www.w3.org/1999/02/22-rdf-syntax-ns#type"]["outbound"][0] != null ? inboundData["value"]["http://www.w3.org/1999/02/22-rdf-syntax-ns#type"]["outbound"][0]["uri"].ToString() : "",
                            LinkType = new KeyValuePair<string, string>
                            (
                                linkType,
                                allLinks.GetValueOrDefault(linkType)
                            )
                        });
                    }

                //Looping Version for outound data from PIDURI responce
                if (linksData["https://pid.bayer.com/kos/19050/hasVersions"] != null && linksData["https://pid.bayer.com/kos/19050/hasVersions"]["outbound"].Count() > 0)
                {
                    foreach (var outboundData in linksData["https://pid.bayer.com/kos/19050/hasVersions"]["outbound"])
                    {
                        string outboundTarget = outboundData["value"]["http://pid.bayer.com/kos/19014/hasPID"] != null ? outboundData["value"]["http://pid.bayer.com/kos/19014/hasPID"]["uri"].ToString() : "";
                        targetURIs.Add(outboundTarget);

                        mapLinks.Add(new MapLinkTO()
                        {
                            IsVersionLink = true,
                            Outbound = true,
                            Source = sourcePIDUri,
                            SourceName = sourceName,
                            SourceType = sourceType,
                            Target = outboundTarget,
                            TargetName = "",
                            TargetType = "",
                            LinkType = new KeyValuePair<string, string>
                            (
                                "https://pid.bayer.com/kos/19050/hasVersion",
                                outboundData["value"]["https://pid.bayer.com/kos/19050/hasVersion"] != null ? outboundData["value"]["https://pid.bayer.com/kos/19050/hasVersion"]["value"].ToString() : ""
                            )
                        });
                    }
                }

                //Looping Version for inbound data from PIDURI responce
                if (linksData["https://pid.bayer.com/kos/19050/hasVersions"] != null && linksData["https://pid.bayer.com/kos/19050/hasVersions"]["inbound"].Count() > 0)
                {
                    foreach (var inboundData in linksData["https://pid.bayer.com/kos/19050/hasVersions"]["inbound"])
                    {
                        var inboundTarget = inboundData["value"]["http://pid.bayer.com/kos/19014/hasPID"] != null ? inboundData["value"]["http://pid.bayer.com/kos/19014/hasPID"]["uri"].ToString() : "";
                        targetURIs.Add(inboundTarget);

                        mapLinks.Add(new MapLinkTO()
                        {
                            IsVersionLink = true,
                            Outbound = false,
                            Source = sourcePIDUri,
                            SourceName = sourceName,
                            SourceType = sourceType,
                            Target = inboundTarget,
                            TargetName = "",
                            TargetType = "",
                            LinkType = new KeyValuePair<string, string>
                            (
                                "https://pid.bayer.com/kos/19050/hasVersion",
                                inboundData["value"]["https://pid.bayer.com/kos/19050/hasVersion"]["value"] != null ? inboundData["value"]["https://pid.bayer.com/kos/19050/hasVersion"]["value"].ToString() : ""
                            )
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Exception StackTrace: " + ex.StackTrace + "Exception Message: " + ex.Message);
            }

            MapLinksAndTargetDTO mapLinksAndTargetDTO = new MapLinksAndTargetDTO() { MapLinks = mapLinks, TargetURIs = targetURIs };

            return mapLinksAndTargetDTO;
        }

        /// <summary>
        /// Get Target Name and Type From Search Service
        /// </summary>
        /// <param name="TargetPIDUris"></param>
        /// <returns></returns>
        public async Task<List<TargetDTO>> GetTargetNameAndTypeFromSearchService(List<string> TargetPIDUris)
        {
            List<TargetDTO> responceTarget = new List<TargetDTO>();
            var response = await _remoteSearchService.GetDocumentsByIds(TargetPIDUris);

            if (response.Count > 0)
            {
                foreach (var pidUri in TargetPIDUris)
                {
                    string encodedPidUri = HttpUtility.UrlEncode(pidUri);
                    var resourceData = response[encodedPidUri];

                    foreach (var data in resourceData)
                    {
                        if (data != null)
                        {
                            responceTarget.Add(new TargetDTO() { 
                                TargetPIDUri = pidUri, 
                                TargetName = data[_serviceUrl + "kos/19050/hasLabel"]["outbound"][0]["value"].ToString(), 
                                TargetType = data["http://www.w3.org/1999/02/22-rdf-syntax-ns#type"]["outbound"][0]["uri"].ToString() });
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
        public string GenerateShortName(string longName)
        {
            string[] segments = longName.Split(" ");
            for (var i = 0; i < segments.Length; i++)
            {
                if (segments[i].Length > 3)
                {
                    segments[i] = segments[i].Substring(0, 3);
                }
            }
            string shortName = string.Join("-", segments);
            return shortName;
        }
    }
}
