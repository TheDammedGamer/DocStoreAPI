
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
            var entity = this.GetById(id);
            this.Delete(entity);
        }

        public void Edit(AccessControlEntity entity)
        {
            GetById(entity.Id);
            _context.AcessControlEntity.Update(entity);
        }

        public AccessControlEntity GetById(int id)
        {
            var entity = _context.AcessControlEntity.FirstOrDefault(g => g.Id == id);

            if (string.IsNullOrWhiteSpace(entity.BusinessArea) || string.IsNullOrWhiteSpace(entity.Group))
                throw new Exception("Unable to Find Dcoument With Specified ID");

            return entity;
        }

        public IEnumerable<AccessControlEntity> List()
        {
            return _context.AcessControlEntity.AsEnumerable();
        }

        public IEnumerable<AccessControlEntity> List(Expression<Func<AccessControlEntity, bool>> predicate)
        {
            return _context.AcessControlEntity.Where(predicate).AsEnumerable();
        }
    }
}

