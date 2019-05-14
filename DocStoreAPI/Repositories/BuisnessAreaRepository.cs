using DocStoreAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocStoreAPI.Repositories
{
    public class BuisnessAreaRepository : BaseRepository
    {
        private readonly DocStoreContext _context;
        private readonly ILogger _logger;

        public BuisnessAreaRepository(ILogger<BuisnessAreaRepository> logger, DocStoreContext docStoreContext)
        {
            _logger = logger;
            _context = docStoreContext;
        }

        public void Add(BuisnessAreaEntity entity)
        {
            _context.BuisnessAreas.Add(entity);
        }

        public void Delete(BuisnessAreaEntity entity)
        {
            _context.BuisnessAreas.Remove(entity);
        }

        public void DeleteById(int id)
        {
            var entity = this.GetById(id);
            this.Delete(entity);
        }

        public void Edit(BuisnessAreaEntity entity)
        {
            GetById(entity.Id);
            _context.BuisnessAreas.Update(entity);
        }

        public BuisnessAreaEntity GetById(int id)
        {
            var entity = _context.BuisnessAreas.FirstOrDefault(g => g.Id == id);

            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new Exception("Unable to Find Buisness Area With Specified ID");

            return entity;
        }

        public IEnumerable<BuisnessAreaEntity> List()
        {
            return _context.BuisnessAreas.AsEnumerable();
        }

        public IEnumerable<BuisnessAreaEntity> List(Expression<Func<BuisnessAreaEntity, bool>> predicate)
        {
            return _context.BuisnessAreas.Where(predicate).AsEnumerable();
        }
    }
}