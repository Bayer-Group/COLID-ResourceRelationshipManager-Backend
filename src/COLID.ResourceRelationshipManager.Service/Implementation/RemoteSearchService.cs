using COLID.Identity.Extensions;
using COLID.Identity.Services;
using COLID.ResourceRelationshipManager.Services.Configuration;
using COLID.ResourceRelationshipManager.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COLID.ResourceRelationshipManager.Services.Implementation
{
    public class RemoteSearchService : IRemoteSearchService
    {
        private readonly CancellationToken _cancellationToken;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ITokenService<ColidSearchServiceTokenOptions> _tokenService;
        private readonly ILogger<RemoteSearchService> _logger;
        private readonly bool _bypassProxy;
        private readonly string SearchService_Document_Api;

        public RemoteSearchService(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ITokenService<ColidSearchServiceTokenOptions> tokenService,
            ILogger<RemoteSearchService> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _cancellationToken = httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
            _logger = logger;
            _bypassProxy = _configuration.GetValue<bool>("BypassProxy");
            var baseUri = _configuration.GetConnectionString("searchServiceReindexUrl");
            SearchService_Document_Api = $"{baseUri}/api/documentsByIds";
        }

        /// <summary>
        /// Aquire Token And Send To Search Service
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="httpMethod"></param>
        /// <param name="endpointUrl"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> AquireTokenAndSendToSearchService(HttpClient httpClient, HttpMethod httpMethod, string endpointUrl, object requestBody)
        {
            try
            {
                var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                var response = await httpClient.SendRequestWithOptionsAsync(httpMethod, endpointUrl,
                    requestBody, accessToken, _cancellationToken);
                return response;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Something went wrong in AquireTokenAndSendToRegistrationService", ex); ;
            }
        }

        /// <summary>
        /// Get Documents By PIDUris
        /// </summary>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, IEnumerable<JObject>>> GetDocumentsByIds(IEnumerable<string> identifiers)
        {
            var result = new Dictionary<string, IEnumerable<JObject>>();
            var chunkSize = 100; // Adjust the chunk size as needed
            var identifierChunks = identifiers.Select((id, index) => new { id, index })
                                              .GroupBy(x => x.index / chunkSize)
                                              .Select(g => g.Select(x => x.id));

            foreach (var chunk in identifierChunks)
            {
                using (var client = (_bypassProxy ? _clientFactory.CreateClient("NoProxy") : _clientFactory.CreateClient()))
                {
                    try
                    {
                        string jsonobject = JsonConvert.SerializeObject(chunk);
                        StringContent content = new StringContent(jsonobject, Encoding.UTF8, "application/json");

                        var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                        _logger.LogInformation("Sending request to search service to get documents by id: " + JsonConvert.SerializeObject(content) + " and identifiers " + JsonConvert.SerializeObject(chunk));
                        HttpResponseMessage response = await client.PostAsync(SearchService_Document_Api, content);
                        content.Dispose();
                        response.EnsureSuccessStatusCode();
                        var chunkResult = JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<JObject>>>(response.Content.ReadAsStringAsync().Result, new VersionConverter());

                        foreach (var kvp in chunkResult)
                        {
                            if (result.ContainsKey(kvp.Key))
                            {
                                result[kvp.Key] = result[kvp.Key].Concat(kvp.Value);
                            }
                            else
                            {
                                result[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError("An error occurred while passing the search request to the search service GetDocumentsByIds.", ex);
                        throw;
                    }
                }
            }

            return result;
        }
    }
}
