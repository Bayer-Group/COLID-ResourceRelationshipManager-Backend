using COLID.Identity.Extensions;
using COLID.Identity.Services;
using COLID.ResourceRelationshipManager.Services.Configuration;
using COLID.ResourceRelationshipManager.Services.Interface;
using CorrelationId.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
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
            ICorrelationContextAccessor correlationContextAccessor,
            ITokenService<ColidSearchServiceTokenOptions> tokenService,
            ILogger<RemoteSearchService> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _tokenService = tokenService;
            _correlationContextAccessor = correlationContextAccessor;
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
                    requestBody, accessToken, _cancellationToken, _correlationContextAccessor.CorrelationContext);
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
            using (var client = (_bypassProxy ? _clientFactory.CreateClient("NoProxy") : _clientFactory.CreateClient()))
            {
                try
                {
                    // Encode the searchRequest into a JSON object for sending
                    string jsonobject = JsonConvert.SerializeObject(identifiers);
                    StringContent content = new StringContent(jsonobject, Encoding.UTF8, "application/json");

                    //Fetch token for search service
                    var accessToken = await _tokenService.GetAccessTokenForWebApiAsync();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    _logger.LogInformation("Sending request to search service to get documents by id: " + JsonConvert.SerializeObject(content) + " and identifiers " + JsonConvert.SerializeObject(identifiers));
                    // Post the JSON object to the SearchService endpoint
                    HttpResponseMessage response = await client.PostAsync(SearchService_Document_Api, content);
                    content.Dispose();
                    response.EnsureSuccessStatusCode();
                    var result = JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<JObject>>>(response.Content.ReadAsStringAsync().Result, new VersionConverter());
                    return result;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("An error occurred while passing the search request to the search service GetDocumentsByIds.", ex);
                    throw ex;
                }
            }
        }
    }
}
