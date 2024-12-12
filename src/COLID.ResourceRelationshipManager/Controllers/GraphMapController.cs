using COLID.Graph.TripleStore.DataModels.Base;
using COLID.Identity.Requirements;
using COLID.ResourceRelationshipManager.Common.DataModels;
using COLID.ResourceRelationshipManager.Common.DataModels.Entity;
using COLID.ResourceRelationshipManager.Common.DataModels.NewTOs;
using COLID.ResourceRelationshipManager.Common.DataModels.RequestDTOs;
using COLID.ResourceRelationshipManager.Common.DataModels.Resource;
using COLID.ResourceRelationshipManager.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [ProducesResponseType(typeof(List<RelationMap>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<RelationMap>), StatusCodes.Status500InternalServerError)]
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
        /// Returns a list of all created maps with piduri
        /// </summary>
        /// <returns>A list of all created graphs</returns>
        /// <response code="200">Returns the list of graphs. If there are no graphs, an empty list is returned.</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("All/pidURIs")]
        [ProducesResponseType(typeof(List<RelationMap>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<RelationMap>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllGraphMapsWithPIDUri()
        {
            var mapProxyDTOs = await _graphMapService.GetAllMapProxyDTOs();
            if (mapProxyDTOs?.Count == 0)
            {
                return NotFound();
            }

            return Ok(mapProxyDTOs);
        }

        /// <summary>
        /// Returns a list of all graphs created by the user
        /// </summary>
        /// <returns>A list of all graphs created by the user</returns>
        /// <response code="200">Returns the list of graphs. If there are no graphs, an empty list is returned.</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("GraphsForUser")]
        [ProducesResponseType(typeof(List<RelationMap>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<RelationMap>), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(List<RelationMap>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<RelationMap>), StatusCodes.Status500InternalServerError)]
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
        /// Fetch the resource(s) information
        /// </summary>
        /// <param name="resourceUris"></param>
        /// <returns></returns>
        /// <response code="200">Returns the resources search for</response>
        /// <response code="400">If the resource is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpPost("FetchResources")]
        [RequestSizeLimit(30_000_000)]
        [ProducesResponseType(typeof(List<MapNodeTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<MapNodeTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<MapNodeTO>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetResources(IList<Uri> resourceUris)
        {
            var graphMap = await _graphMapService.GetResources(resourceUris);
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
        public async Task<IActionResult> GetMapPage(GraphMapSearchDTO graphMapSearchDTO, string offset)
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
        public async Task<IActionResult> ManageResourceLinking(IList<LinkResourceTypeDTOV2> linkResourceTypes)
        {
            var result = await _graphMapService.ManageResourceLinking(linkResourceTypes);
            if (result?.Count == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Creates a Relation Map
        /// </summary>
        /// <param name="graphMapV2SaveDto">The new graph to create</param>
        /// <returns>A newly created graph</returns>
        /// <response code="201">Returns the newly created graph</response>
        /// <response code="400">If the graph is invalid</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpPut("SaveRelationMap")]
        [ProducesResponseType(typeof(RelationMap), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RelationMap), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RelationMap), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveRelationMap(GraphMapV2SaveDto graphMapV2SaveDto)
        {
            var result = await _graphMapService.SaveRelationMap(graphMapV2SaveDto);
            return Ok(result);
        }

        /// <summary>
        /// Delete the Relation Map
        /// </summary>
        /// <param name="relationMapId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteRelationMap")]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRelationMap(string relationMapId)
        {
            var relationMap = await _graphMapService.GetPlainRelationMapById(relationMapId);

            if (relationMap == null)
            {
                return NotFound();
            }

            bool deletionSuccessful = await _graphMapService.DeleteRelationMap(relationMap);

            if (!deletionSuccessful)
            {
                return new ObjectResult("You don't have the rights to delete this map")
                {
                    StatusCode = 403
                };
            }

            return Ok(relationMap);
        }

        /// <summary>
        /// Delete the Relation Map As An Admin
        /// </summary>
        /// <param name="relationMapId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteRelationMapAsSuperAdmin")]
        [Authorize(Policy = nameof(SuperadministratorRequirement))]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Entity), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRelationMapAsSuperAdmin(string relationMapId)
        {
            var relationMap = await _graphMapService.GetPlainRelationMapById(relationMapId);

            if (relationMap == null)
            {
                return NotFound();
            }

            await _graphMapService.DeleteRelationMap(relationMap, true);

            return Ok(relationMap);
        }

        /// <summary>
        /// Returns the relation graph of the given Id.
        /// </summary>
        /// <param name="relationMapId">The Id of a relation map.</param>
        /// <returns>A relation graph</returns>
        /// <response code="200">Returns the graph of the given Id</response>
        /// <response code="404">If no graph exists with the given Id</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("GetRelationMapById")]
        [ProducesResponseType(typeof(GraphMapTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GraphMapTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GraphMapTO), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRelationMapById(string relationMapId)
        {
            var relationMap = await _graphMapService.GetRelationMapById(relationMapId);
            if (relationMap == null)
            {
                return NotFound();
            }
            return Ok(relationMap);
        }

        /// <summary>
        /// Filter and Fetch the resource(s) information
        /// </summary>
        /// <param name="resourceUris"></param>
        /// <returns></returns>
        /// <response code="200">Returns the resources search for</response>
        /// <response code="400">If the resource is not found</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpPost("FetchResources/Filter")]
        [RequestSizeLimit(30_000_000)]
        [ProducesResponseType(typeof(List<MapNodeTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<MapNodeTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<MapNodeTO>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredResources(LinkResourcesFilterDTO resourceFilterRequest)
        {
            var filteredResources = await _graphMapService.GetFilteredResources(resourceFilterRequest);
            if (filteredResources?.Count == 0)
            {
                return NotFound();
            }
            return Ok(filteredResources);
        }
    }
}
