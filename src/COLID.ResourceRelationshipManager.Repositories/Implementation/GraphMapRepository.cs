using COLID.Exception.Models;
using COLID.ResourceRelationshipManager.Common.DataModels.Entity;
using COLID.ResourceRelationshipManager.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace COLID.ResourceRelationshipManager.Repositories.Implementation
{
    public class GraphMapRepository : IGraphMapRepository
    {
        private readonly ResourceRelationshipManagerContext _db;
        public GraphMapRepository(ResourceRelationshipManagerContext resourceRelationshipManagerContext)
        {
            _db = resourceRelationshipManagerContext;
        }

        public async Task<IList<RelationMap>> GetAllGraphMaps(int limit, int offset)
        {
            var relationMaps = await _db.RelationMap
                .Include(r => r.Nodes)
                .OrderByDescending(o => o.ModifiedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            return relationMaps;
        }

        public async Task<IList<RelationMap>> GetPageGraphMaps(GraphMapSearchDTO graphMapSearchDTO, int offset)
        {
            Expression<Func<RelationMap, IComparable>> keySelector = keySelector = (o => o.Name);
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
                return await _db.RelationMap
                    .Where(o => o.Name.ToLower().Contains(graphMapSearchDTO.nameFilter.Trim().ToLower()))
                    .Include(r => r.Nodes)
                    .OrderBy(keySelector)
                    .Skip(offset)
                    .Take(graphMapSearchDTO.batchSize)
                    .ToListAsync();
            else
                return await _db.RelationMap
                .Where(o => o.Name.ToLower().Contains(graphMapSearchDTO.nameFilter.Trim().ToLower()))
                .Include(r => r.Nodes)
                .OrderByDescending(keySelector)
                .Skip(offset)
                .Take(graphMapSearchDTO.batchSize)
                .ToListAsync();
        }

        public async Task<IList<RelationMap>> GetGraphMapsForUser(string userId, int limit, int offset)
        {
            var relationMap = await _db.RelationMap
                .Where(g => g.ModifiedBy == userId)
                .OrderByDescending(o => o.ModifiedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            return relationMap;
        }

        public async Task<IList<RelationMap>> GetGraphMapForResource(Uri pidUri)
        {
            var relationMap = await _db.RelationMap
                .Where(x=> x.Nodes.Any(x=> x.PIDUri == pidUri))
                .Include(r => r.Nodes)
                .ToListAsync();
            return relationMap;
        }
        
        public async Task<RelationMap> GetRelationMapByName(string relationMapName)
        {
            var relationMap = await _db.RelationMap
                .Where(g => g.Name == (string.IsNullOrEmpty(relationMapName) ? string.Empty : relationMapName.Trim()))
                .FirstOrDefaultAsync();

            return relationMap;
        }

        public async Task<Guid?> SaveRelationMap(RelationMap relationMapReqDto)
        {
            if (relationMapReqDto.Id == null || relationMapReqDto.Id == Guid.Empty)
            {
                _db.RelationMap.Add(relationMapReqDto);
                await _db.SaveChangesAsync();
            }
            else
            {
                var currentRelationMap = _db.RelationMap
                .Include(x => x.Nodes)
                .FirstOrDefault(x => x.Id == relationMapReqDto.Id);

                if (currentRelationMap.ModifiedBy == relationMapReqDto.ModifiedBy)
                {
                    await this.DeleteNodesByRelationMapId(currentRelationMap.Id); // removing existing nodes

                    foreach (var node in relationMapReqDto.Nodes)
                    {
                        // adding updated nodes
                        currentRelationMap.Nodes.Add(new Nodes() { PIDUri = node.PIDUri, RelationMapId = currentRelationMap.Id, xPosition = node.xPosition, yPosition = node.yPosition });
                    }
                    currentRelationMap.Description = relationMapReqDto.Description;
                    currentRelationMap.Name = relationMapReqDto.Name;
                    currentRelationMap.ModifiedAt = relationMapReqDto.ModifiedAt;

                    _db.RelationMap.Update(currentRelationMap);
                    await _db.SaveChangesAsync();
                } else
                {
                    throw new BusinessException(message: $"Forbidden Error!");
                }
            }
            Guid? lastInsertedRelationMapId = relationMapReqDto.Id;
            return lastInsertedRelationMapId;
        }
        
        public async Task<RelationMap> GetRelationMapById(string relationMapId)
        {
            var relationMap = await _db.RelationMap
                .Where(r => r.Id == Guid.Parse(relationMapId))
                .Include(r => r.Nodes)
                .FirstOrDefaultAsync();

            return relationMap;
        }

        public async Task DeleteNodesByRelationMapId(Guid? relationMapId)
        {
            var deleteNodes = await _db.Nodes.Where(n => n.RelationMapId == relationMapId).ToListAsync();
            _db.RemoveRange(deleteNodes);
            _db.SaveChanges();
        }

        public async Task DeleteRelationMap(RelationMap relationMap)
        {
            _db.Remove(relationMap);
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