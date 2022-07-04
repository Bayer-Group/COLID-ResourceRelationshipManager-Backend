using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COLID.ResourceRelationshipManager.Services.Interface;
using COLID.ResourceRelationshipManager.Common.DataModels;
using COLID.Graph.TripleStore.DataModels.Base;

namespace COLID.ResourceRelationshipManager.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GraphMapController : ControllerBase
    {
        private readonly IGraphMapService _graphMapService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphMapService"></param>
        public GraphMapController(IGraphMapService graphMapService)
        {
            _graphMapService = graphMapService;
        }

        /// <summary>
        /// Returns a list of all created graphs
        /// </summary>
        /// <returns>A list of all created graphs</returns>
        /// <response code="200">Returns the list of graphs. If there are no graphs, an empty list is returned.</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("All")]
        [ProducesResponseType(typeof(List<GraphMap>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<GraphMap>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllGraphMaps(int limit = 10, int offset = 0)
        {
            var graphMaps = await _graphMapService.GetAllGraphMaps(limit, offset);
            if (graphMaps?.Count == 0)
            {
                return NotFound();
            }
            return Ok(graphMaps);
        }

        /// <summary>
        /// Returns a list of all graphs created by the user
        /// </summary>
        /// <returns>A list of all graphs created by the user</returns>
        /// <response code="200">Returns the list of graphs. If there are no graphs, an empty list is returned.</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("GraphsForUser")]
        [ProducesResponseType(typeof(List<GraphMap>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<GraphMap>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGraphMapForUser(string userName, int limit = 10, int offset = 0)
        {
            //TODO: Check if username passed here is the user requesting
            var graphMaps = await _graphMapService.GetGraphMapsForUser(userName, limit, offset);
            if (graphMaps?.Count == 0)
            {
                return NotFound();
            }
            return Ok(graphMaps);
        }

        /// <summary>
        /// Returns a list of all graphs containing particular resource
        /// </summary>
        /// <returns>A list of all graphs containing particular Node</returns>
        /// <response code="200">Returns the list of graphs. If there are no graphs, an empty list is returned.</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("GraphsForResource")]
        [ProducesResponseType(typeof(List<GraphMap>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<GraphMap>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGraphMapForResource(Uri pidUri)
        {
            var graphMaps = await _graphMapService.GetGraphMapForResource(pidUri);
            if (graphMaps?.Count == 0)
            {
                return NotFound();
            }
            return Ok(graphMaps);
        }

        /// <summary>
        /// Returns the graph of the given Id.
        /// </summary>
        /// <param name="id">The Id of a graph.</param>
        /// <returns>A graph</returns>
        /// <response code="200">Returns the graph of the given Id</response>
        /// <response code="404">If no graph exists with the given Id</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet]
        [ProducesResponseType(typeof(GraphMap), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GraphMap), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GraphMap), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGraphMapById(string id)
        {
            var graphMap = await _graphMapService.GetGraphMapById(id);
            if (graphMap == null)
            {
                return NotFound();
            }
            return Ok(graphMap);
        }

        /// <summary>
        /// Creates a graph.
        /// </summary>
        /// <param name="graphMap">The new graph to create</param>
        /// <returns>A newly created graph</returns>
        /// <response code="201">Returns the newly created graph</response>
        /// <response code="400">If the graph is invalid</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpPut]
        [ProducesResponseType(typeof(GraphMap), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GraphMap), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GraphMap), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveGraphMap(GraphMap graphMap)
        {
            await _graphMapService.SaveGraphMap(graphMap);
            return Ok(graphMap);
        }

        /// <summary>
        /// Fetch the resource(s) information
        /// </summary>
        /// <param name="resourceUris"></param>
        /// <returns></returns>
        /// <response code="200">Returns the resources search for</response>
        /// <response code="400">If the resource is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpPost("FetchResources")]
        [ProducesResponseType(typeof(List<ResourceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResourceDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<ResourceDTO>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetResources(List<Uri> resourceUris)
        {
            var graphMap = await _graphMapService.GetResourcesFromTripleStore(resourceUris);
            if (graphMap?.Count == 0)
            {
                return NotFound();
            }
            return Ok(graphMap);
        }

        /// <summary>
        /// Fetch the page of graph maps
        /// </summary>
        /// <param name="graphMapSearchDTO"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <response code="200">Returns the resources search for</response>
        /// <response code="400">If the resource is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpPost("Page")]
        [ProducesResponseType(typeof(List<ResourceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResourceDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<ResourceDTO>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMapPage(Common.DataModels.Entity.GraphMapSearchDTO graphMapSearchDTO, string offset)
        {
            int offsetNr = Int32.Parse(offset);
            var graphMaps = await _graphMapService.GetPageGraphMaps(graphMapSearchDTO, offsetNr);
            if (graphMaps?.Count == 0)
            {
                return Ok(graphMaps);
            }
            return Ok(graphMaps);
        }


        //1. link => ResourceTypes => produce possible resourcetype relation
        //2. Publish new links
        /// <summary>
        /// Produce possible resourcetype relation
        /// </summary>
        /// <param name="linkTypeRequest"></param>
        /// <returns></returns>
        /// <response code="200">Returns the resources search for</response>
        /// <response code="400">If the resource is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        ///
        [HttpPost("LinkResourceTypes")]
        [ProducesResponseType(typeof(List<LinkResourceTypeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<LinkResourceTypeDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<LinkResourceTypeDTO>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LinkResourceTypes([FromBody] LinkTypeRequestDTO linkTypeRequest)
        { 
            var graphMap = await _graphMapService.GetLinkResourceTypes(linkTypeRequest.SourceUri, linkTypeRequest.TargetUri);
            if (graphMap?.Count == 0)
            {
                return NotFound();
            }
            return Ok(graphMap);
        }

        /// <summary>
        /// Link or unlink resources from each other based on the action
        /// </summary>
        /// <param name="linkResourceTypes"></param>
        /// <returns></returns>
        [HttpPost("ManageResourceLinking")]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ManageResourceLinking(List<LinkResourceTypeDTO> linkResourceTypes)
        {
            var result = await _graphMapService.ManageResourceLinking(linkResourceTypes);
            if (result?.Count == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete the graph
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteGraphMap(string id)
        {
            var graphMap = await _graphMapService.GetGraphMapById(id);
            if(graphMap == null)
            {
                return NotFound();
            }
            
            await _graphMapService.DeleteGraphMap(graphMap);
            return Ok(graphMap);
        }
    }
}
