using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using COLID.ResourceRelationshipManager.Common.DataModels;
using COLID.ResourceRelationshipManager.Repositories.Interface;
using System.Threading.Tasks;
using COLID.ResourceRelationshipManager.Common.DataModels.Entity;
using System.Collections;
using System.Linq.Expressions;

namespace COLID.ResourceRelationshipManager.Repositories.Implementation
{
    public class GraphMapRepository : IGraphMapRepository
    {
        private readonly ResourceRelationshipManagerContext _db;
        public GraphMapRepository(ResourceRelationshipManagerContext resourceRelationshipManagerContext)
        {
            _db = resourceRelationshipManagerContext;
        }
        public async Task<IList<GraphMap>> GetAllGraphMaps(int limit, int offset)
        {
            var graphMaps = await _db.GraphMaps
                .OrderByDescending(o => o.ModifiedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            return graphMaps;
        }

        public async Task<IList<GraphMap>> GetPageGraphMaps(GraphMapSearchDTO graphMapSearchDTO, int offset)
        {
            Expression<Func<GraphMap, IComparable>> keySelector = keySelector = (o => o.Name);
            if (graphMapSearchDTO.sortKey.Equals("modifiedBy"))
                keySelector = o => o.ModifiedBy;
            if (graphMapSearchDTO.sortKey.Equals("modifiedAt"))
                keySelector = o => o.ModifiedAt.ToString();
           
            //(graphMapSearchDTO.sortKey.Equals("modifiedBy") ? (Expression<Func<GraphMap, DateTime>>)(o => o.ModifiedBy) : (Expression<Func<GraphMap, DateTime>>)(o => o.ModifiedAt));
            //IComparer<IComparable> comparer = graphMapSearchDTO.sortType == "asc" ? Comparer<IComparable>.Create((a, b) => a.CompareTo(b)) : 
            //    (graphMapSearchDTO.sortType == "des" ? Comparer<IComparable>.Create((a, b) => b.CompareTo(a)) : Comparer<IComparable>.Create((a, b) => 0));

            //if (comparer.Compare("name1", "name2")==0) 
            //    return new List<GraphMap>();

            if (graphMapSearchDTO.sortType.Equals("asc"))
                return await _db.GraphMaps
                    .Where(o => o.Name.ToLower().Contains(graphMapSearchDTO.nameFilter.Trim().ToLower()))
                    .OrderBy(keySelector)
                    .Skip(offset)
                    .Take(graphMapSearchDTO.batchSize)
                    .ToListAsync();
            else
                return await _db.GraphMaps
                .Where(o => o.Name.ToLower().Contains(graphMapSearchDTO.nameFilter.Trim().ToLower()))
                .OrderByDescending(keySelector)
                .Skip(offset)
                .Take(graphMapSearchDTO.batchSize)
                .ToListAsync();
        }

        public async Task<IList<GraphMap>> GetGraphMapsForUser(string userId, int limit, int offset)
        {
            var graphMaps = await _db.GraphMaps
                .Where(g => g.ModifiedBy == userId)
                .OrderByDescending(o => o.ModifiedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            return graphMaps;
        }
        public async Task<IList<GraphMap>> GetGraphMapForResource(Uri pidUri)
        {
            var graphMaps = await _db.GraphMaps
                .Where(x=> x.MapNodes.Any(x=> x.PidUri == pidUri))
                .ToListAsync();

            return graphMaps;
        }
        public async Task<GraphMap> GetGraphMapById(string mapId)
        {
            var graphMap = await _db.GraphMaps
                .Where(g => g.GraphMapId == Guid.Parse(mapId))
                .Include(x => x.MapNodes).ThenInclude(x => x.Links).ThenInclude(x => x.Type) //include maplinkinfo
                .Include(x => x.MapNodes).ThenInclude(x => x.Links).ThenInclude(x => x.StartNode) //include maplinkinfo
                .Include(x => x.MapNodes).ThenInclude(x => x.Links).ThenInclude(x => x.EndNode) //include maplinkinfo
                .Include(x => x.MapLinks).ThenInclude(x => x.Name)
                .FirstOrDefaultAsync();

            //loadEntities(graphMap?.rootNode?.Links);
            return graphMap;
        }
        public async Task<GraphMap> GetGraphMapByName(string mapName)
        {
            var graphMap = await _db.GraphMaps
                .Where(g => g.Name == (string.IsNullOrEmpty(mapName) ? string.Empty : mapName.Trim()))
                .FirstOrDefaultAsync();

            //loadEntities(graphMap?.rootNode?.Links);
            return graphMap;
        }
        public async Task SaveGraphMap(GraphMap graphMap)
        {
            //TODO: Ensure that all objects in the hierarchy are being saved (recursion?)
            if (graphMap.GraphMapId == null || graphMap.GraphMapId == Guid.Empty)
                _db.GraphMaps.Add(graphMap);
            else
                _db.GraphMaps.Update(graphMap);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteGraphMap(GraphMap graphMap)
        {
            _db.Remove(graphMap);
            await _db.SaveChangesAsync();
        }
        //private void loadEntities(ICollection<MapLink> links)
        //{
        //    if (links == null)
        //        return;

        //    foreach (var link in links)
        //    {
        //        _db.Entry<MapLink>(link).Reference(e => e.targetNode).Load();
        //        var node = link.targetNode;
        //        if (node == null) break;
        //        _db.Entry<MapNode>(node).Collection(e => e.Links).Load();
        //        if (node.Links == null || node.Links.Count == 0) break;
        //        loadEntities(node.Links);
        //    }
        //}
    }
}