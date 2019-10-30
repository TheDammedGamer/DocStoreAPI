using DocStore.Server.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocStore.Server.Repositories;
using DocStore.Shared.Models;

namespace DocStore.Server.Repositories
{
    public class GroupRepository : BaseRepository
    {
        private readonly DocStoreContext _context;
        private readonly ILogger _logger;

        public GroupRepository(ILogger<GroupRepository> logger, DocStoreContext docStoreContext)
        {
            _logger = logger;
            _context = docStoreContext;
        }

        public void Add(GroupEntity entity)
        {
            _context.GroupEntities.Add(entity);
        }

        public void Delete(GroupEntity entity)
        {
            _context.GroupEntities.Remove(entity);
        }

        public void DeleteById(int id)
        {
            var entity = GetById(id);
            Delete(entity);
        }

        public void Edit(GroupEntity entity)
        {
            GetById(entity.Id);
            _context.GroupEntities.Update(entity);
        }

        public GroupEntity GetById(int id)
        {
            return _context.GroupEntities.FirstOrDefault(g => g.Id == id);
        }

        public GroupEntity GetByName(string name)
        {
            return _context.GroupEntities.FirstOrDefault(g => g.Name == name);
        }

        public IEnumerable<GroupEntity> List()
        {
            return _context.GroupEntities.AsEnumerable();
        }

        public IEnumerable<GroupEntity> List(Expression<Func<GroupEntity, bool>> predicate)
        {
            return _context.GroupEntities.Where(predicate).AsEnumerable();
        }

        public IEnumerable<GroupEntity> ListP(out int totalPages, int perPage, int page)
        {
            int totalRecords = _context.GroupEntities.Count();

            totalPages = 0;
            if (totalRecords > 0)
                totalPages = (((totalRecords - 1) / perPage) + 1);
            IEnumerable<GroupEntity> entities;
            if (page == 0)
                entities = _context.GroupEntities.Take(perPage).AsEnumerable();
            else
                entities = _context.GroupEntities.Skip(page * perPage).Take(perPage).AsEnumerable();

            return entities;
        }
    }
}
