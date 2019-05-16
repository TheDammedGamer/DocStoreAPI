
using DocStoreAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocStoreAPI.Repositories
{
    public class AccessRepository : BaseRepository
    {
        private readonly DocStoreContext _context;
        private readonly ILogger _logger;

        public AccessRepository(ILogger<AccessRepository> logger, DocStoreContext docStoreContext)
        {
            _logger = logger;
            _context = docStoreContext;
        }

        public void Add(AccessControlEntity entity)
        {
            _context.AcessControlEntity.Add(entity);
        }

        public void Delete(AccessControlEntity entity)
        {
            _context.AcessControlEntity.Remove(entity);
        }

        public void DeleteById(int id)
        {
            var entity = GetById(id);
            Delete(entity);
        }

        public void Edit(AccessControlEntity entity)
        {
            GetById(entity.Id);
            _context.AcessControlEntity.Update(entity);
        }

        public AccessControlEntity GetById(int id)
        {
            return _context.AcessControlEntity.FirstOrDefault(g => g.Id == id);
        }

        public IEnumerable<AccessControlEntity> List()
        {
            return _context.AcessControlEntity.AsEnumerable();
        }

        public IEnumerable<AccessControlEntity> List(Expression<Func<AccessControlEntity, bool>> predicate)
        {
            return _context.AcessControlEntity.Where(predicate).AsEnumerable();
        }

        public IEnumerable<AccessControlEntity> ListP(out int totalPages, int perPage, int page)
        {
            int totalRecords = _context.AcessControlEntity.Count();

            totalPages = 0;
            if (totalRecords > 0)
                totalPages = (((totalRecords - 1) / perPage) + 1);
            IEnumerable<AccessControlEntity> entities;
            if (page == 0)
                entities = _context.AcessControlEntity.Take(perPage).AsEnumerable();
            else
                entities = _context.AcessControlEntity.Skip(page * perPage).Take(perPage).AsEnumerable();

            return entities;
        }

    }
}

