using DocStoreAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocStoreAPI.Repositories
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
            var entity = this.GetById(id);
            this.Delete(entity);
        }

        public void Edit(GroupEntity entity)
        {
            GetById(entity.Id);
            _context.GroupEntities.Update(entity);
        }

        public GroupEntity GetById(int id)
        {
            var entity = _context.GroupEntities.FirstOrDefault(g => g.Id == id);

            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new Exception("Unable to Find Dcoument With Specified ID");

            return entity;
        }

        public IEnumerable<GroupEntity> List()
        {
            return _context.GroupEntities.AsEnumerable();
        }

        public IEnumerable<GroupEntity> List(Expression<Func<GroupEntity, bool>> predicate)
        {
            return _context.GroupEntities.Where(predicate).AsEnumerable();
        }
    }
}
