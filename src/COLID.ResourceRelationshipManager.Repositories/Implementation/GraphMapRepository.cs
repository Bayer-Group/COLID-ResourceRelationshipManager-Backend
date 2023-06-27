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
            Dictionary<string, dynamic> orderFunctions = new Dictionary<string, dynamic>
            {
                { "description", (Expression<Func<RelationMap, string>>)(x => x.Description) },
                { "nodeCount", (Expression<Func<RelationMap, int>>)(x => x.Nodes.Count) },
                { "date", (Expression<Func<RelationMap, DateTime>>)(x => x.ModifiedAt) },
                { "creator", (Expression<Func<RelationMap, string>>)(x => x.ModifiedBy) }
            };

            IQueryable<RelationMap> query = _db.RelationMap;

            if (graphMapSearchDTO.nameFilter.Trim().Length != 0)
            {
                query = query.Where(m => m.Name.ToLower().Contains(graphMapSearchDTO.nameFilter.Trim().ToLower()));
            }

            query = query.Include(m => m.Nodes);

            if (graphMapSearchDTO.sortType.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                query = Queryable.OrderBy(query, orderFunctions[graphMapSearchDTO.sortKey]);
            }

            if (graphMapSearchDTO.sortType.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query = Queryable.OrderByDescending(query, orderFunctions[graphMapSearchDTO.sortKey]);
            }

            query = query
                .Skip(offset)
                .Take(graphMapSearchDTO.batchSize);

            var result = await query.ToListAsync();

            return result;
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

        public async Task<Guid> SaveRelationMap(RelationMap relationMap, bool isNew)
        {
            if (isNew)
            {
                _db.RelationMap.Add(relationMap);
                await _db.SaveChangesAsync();
            }
            else
            {
                var currentRelationMap = _db.RelationMap
                .Include(x => x.Nodes)
                .FirstOrDefault(x => x.Id == relationMap.Id);

                if (currentRelationMap.ModifiedBy == relationMap.ModifiedBy)
                {
                    await DeleteNodesByRelationMapId(currentRelationMap.Id); // removing existing nodes

                    foreach (var node in relationMap.Nodes)
                    {
                        // adding updated nodes
                        currentRelationMap.Nodes.Add(new Node() { PIDUri = node.PIDUri, RelationMapId = currentRelationMap.Id, xPosition = node.xPosition, yPosition = node.yPosition });
                    }
                    currentRelationMap.Description = relationMap.Description;
                    currentRelationMap.Name = relationMap.Name;
                    currentRelationMap.ModifiedAt = relationMap.ModifiedAt;

                    _db.RelationMap.Update(currentRelationMap);
                    await _db.SaveChangesAsync();
                } else
                {
                    throw new BusinessException(message: $"Forbidden Error!");
                }
            }
            return relationMap.Id;
        }
        
        public async Task<RelationMap> GetRelationMapById(string relationMapId)
        {
            var relationMap = await _db.RelationMap
                .Where(r => r.Id == Guid.Parse(relationMapId))
                .Include(r => r.Nodes)
                .FirstOrDefaultAsync();

            return relationMap;
        }

        public async Task DeleteNodesByRelationMapId(Guid relationMapId)
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
    }
}