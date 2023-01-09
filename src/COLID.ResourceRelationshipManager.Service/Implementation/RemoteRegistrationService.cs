using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using COLID.Identity.Extensions;
using COLID.Identity.Services;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using COLID.Graph.TripleStore.DataModels.Base;
using COLID.Graph.Metadata.DataModels.Metadata;
using COLID.ResourceRelationshipManager.Common.DataModels;
using COLID.ResourceRelationshipManager.Services.Configuration;
using COLID.ResourceRelationshipManager.Services.Interface;
using System.Web;

namespace COLID.ResourceRelationshipManager.Services.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoteRegistrationService : IRemoteRegistrationService
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ITokenService<ColidRegistrationServiceTokenOptions> _tokenService;

        private readonly string RegistrationService_Get_Links_And_Resource_Api;
        private readonly string RegistrationService_Get_Resources_Api;
        private readonly string RegistrationService_Get_Metadata_Api;
        private readonly string RegistrationService_Link_Resources_Api;
        private readonly string RegistrationService_Unlink_Resources_Api;
        private readonly string RegistrationService_Metadata_InstantiableTypes;
        private readonly string RegistrationService_Metadata_LinkTypes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="correlationContextAccessor"></param>
        /// <param name="tokenService"></param>
        public RemoteRegistrationService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ICorrelationContextAccessor correlationContextAccessor,
            ITokenService<ColidRegistrationServiceTokenOptions> tokenService)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _correlationContextAccessor = correlationContextAccessor;
            _cancellationToken = httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;

            var baseUri = _configuration.GetConnectionString("ColidRegistrationServiceUrl");
            RegistrationService_Get_Links_And_Resource_Api = $"{baseUri}/api/v3/resource/linkedresourceList";
            RegistrationService_Get_Resources_Api = $"{baseUri}/api/v3/resource";
            RegistrationService_Get_Metadata_Api = $"{baseUri}/api/v3/metadata";
            RegistrationService_Metadata_InstantiableTypes = $"{baseUri}/api/v3/metadata/instantiableResources";
            RegistrationService_Link_Resources_Api = $"{baseUri}/api/v3/resource/addLink";
            RegistrationService_Unlink_Resources_Api = $"{baseUri}/api/v3/resource/removeLink";
            RegistrationService_Metadata_LinkTypes = $"{baseUri}/api/v3/metadata/linkTypes";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="httpMethod"></param>
        /// <param name="endpointUrl"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> AquireTokenAndSendToRegistrationService(HttpClient httpClient, HttpMethod httpMethod, string endpointUrl, object requestBody)
        {
            var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
            Console.WriteLine("Access token atrs is " + accessToken);
            var response = await httpClient.SendRequestWithOptionsAsync(httpMethod, endpointUrl,
                requestBody, accessToken, _cancellationToken, _correlationContextAccessor.CorrelationContext);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="links"></param>
        /// <param name="resources"></param>
        /// <returns></returns>
        public async Task<List<ResourceCTO>> GetLinksAndResourcesForGraph(List<Uri> resources)
        {
            using var httpClient = _clientFactory.CreateClient();
            var response = await AquireTokenAndSendToRegistrationService(httpClient, HttpMethod.Post,
                $"{RegistrationService_Get_Links_And_Resource_Api}", resources);

            if (!response.IsSuccessStatusCode)
            {
                throw new System.Exception("Something went wrong while fetching resources and their links from RegistrationService");
            }

            var content = JsonConvert.DeserializeObject<List<ResourceCTO>>(response.Content.ReadAsStringAsync().Result, new VersionConverter());

            return content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourcesUris"></param>
        /// <returns></returns>
        public async Task<ResourceCTO> GetResourcesFromGraph(Uri resourcesUri)
        {
            using var httpClient = _clientFactory.CreateClient();
            var response = await AquireTokenAndSendToRegistrationService(httpClient, HttpMethod.Get, $"{RegistrationService_Get_Resources_Api}?pidUri={resourcesUri}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new System.Exception("Something went wrong while fetching resources frmom triplestore in RegistrationService");
            }
            return JsonConvert.DeserializeObject<ResourceCTO>(response.Content.ReadAsStringAsync().Result, new VersionConverter());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="httpMethod"></param>
        /// <param name="endpointUrl"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public async Task<List<MetadataProperty>> GetResourceTypes(Uri resourcesUri)
        {
            using var httpClient = _clientFactory.CreateClient();
            var encodedUri = HttpUtility.UrlEncode(resourcesUri.AbsoluteUri);
            var response = await AquireTokenAndSendToRegistrationService(httpClient,
                HttpMethod.Get,
                $"{RegistrationService_Get_Metadata_Api}?entityType={encodedUri}",
                null);

            if (!response.IsSuccessStatusCode)
            {
                throw new System.Exception("Something went wrong while fetching resourceTypes in RegistrationService");
            }
            return JsonConvert.DeserializeObject<List<MetadataProperty>>(response.Content.ReadAsStringAsync().Result, new VersionConverter());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkResourceDTO"></param>
        /// <returns></returns>
        public async Task<Entity> LinkResource(LinkResourceTypeDTOV2 linkResourceType, string requester)
        {
            using var httpClient = _clientFactory.CreateClient();
            var response = await AquireTokenAndSendToRegistrationService(httpClient,
                HttpMethod.Post,
                 string.Format("{0}?pidUri={1}&linkType={2}&pidUriToLink={3}&requester={4}",
                                RegistrationService_Link_Resources_Api,
                                linkResourceType.SourceUri,
                                linkResourceType.LinkType.Key,
                                linkResourceType.TargetUri,
                                requester
                            )
                , null);

            if (!response.IsSuccessStatusCode)
            {
                throw new System.Exception("Something went wrong while linking two resources in RegistrationService");
            }
            return JsonConvert.DeserializeObject<Entity>(response.Content.ReadAsStringAsync().Result, new VersionConverter());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unlinkResourceDTO"></param>
        /// <returns></returns>
        public async Task<Entity> UnlinkResource(LinkResourceTypeDTOV2 linkResourceType, string requester)
        {
            using var httpClient = _clientFactory.CreateClient();
            var response = await AquireTokenAndSendToRegistrationService(httpClient,
                HttpMethod.Post,
                string.Format("{0}?pidUri={1}&linkType={2}&pidUriToUnLink={3}&returnTargetResource={4}&requester={5}",
                                RegistrationService_Unlink_Resources_Api,
                                linkResourceType.SourceUri,
                                linkResourceType.LinkType.Key,
                                linkResourceType.TargetUri,
                                false,
                                requester
                            )
                , null);

            if (!response.IsSuccessStatusCode)
            {
                throw new System.Exception("Something went wrong while unlinking two resources in RegistrationService");
            }
            return JsonConvert.DeserializeObject<Entity>(response.Content.ReadAsStringAsync().Result, new VersionConverter());
        }
        public async Task<List<Entity>> GetInstantiableLinks(List<Entity> linksInstantiableTypes)
        {
            using (var httpClient = _clientFactory.CreateClient())
            {
                var response = await AquireTokenAndSendToRegistrationService(httpClient,
                    HttpMethod.Post,
                    $"{RegistrationService_Metadata_InstantiableTypes}",
                    linksInstantiableTypes);

                if (!response.IsSuccessStatusCode)
                {
                    throw new System.Exception("Something went wrong while fetching eligible links from RegistrationService");
                }
                return JsonConvert.DeserializeObject<List<Entity>>(response.Content.ReadAsStringAsync().Result, new VersionConverter());
            }
        }

        /// <summary>
        /// Fetch a dictionary of all possible link types.
        /// </summary>
        /// <returns>Dictionary with all possible link types. Key = PID URI of link type, Value = label of that link type.</returns>
        /// <exception cref="System.Exception">In case of errors</exception>
        public async Task<Dictionary<string, string>> GetLinkTypes()
        {
            using(var httpClient = _clientFactory.CreateClient())
            {
                var response = await AquireTokenAndSendToRegistrationService(httpClient,
                    HttpMethod.Get,
                    $"{RegistrationService_Metadata_LinkTypes}",
                    null
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new System.Exception("Something went wrong while fetching the list of all possible link types");
                }
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result, new VersionConverter());
            }
        }
    }
    
}
