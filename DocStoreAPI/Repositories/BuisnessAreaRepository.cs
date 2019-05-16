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
            var entity = GetById(id);
            Delete(entity);
        }

        public void Edit(BuisnessAreaEntity entity)
        {
            GetById(entity.Id);
            _context.BuisnessAreas.Update(entity);
        }

        public BuisnessAreaEntity GetById(int id)
        {
            return _context.BuisnessAreas.FirstOrDefault(g => g.Id == id);
        }

        public BuisnessAreaEntity GetByName(string name)
        {
            return _context.BuisnessAreas.FirstOrDefault(g => g.Name == name);
        }

        public IEnumerable<BuisnessAreaEntity> List()
        {
            return _context.BuisnessAreas.AsEnumerable();
        }

        public IEnumerable<BuisnessAreaEntity> List(Expression<Func<BuisnessAreaEntity, bool>> predicate)
        {
            return _context.BuisnessAreas.Where(predicate).AsEnumerable();
        }

        public IEnumerable<BuisnessAreaEntity> ListP(out int totalPages, int perPage, int page)
        {
            int totalRecords = _context.BuisnessAreas.Count();

            totalPages = 0;
            if (totalRecords > 0)
                totalPages = (((totalRecords - 1) / perPage) + 1);
            IEnumerable<BuisnessAreaEntity> entities;
            if (page == 0)
                entities = _context.BuisnessAreas.Take(perPage).AsEnumerable();
            else
                entities = _context.BuisnessAreas.Skip(page * perPage).Take(perPage).AsEnumerable();

            return entities;
        }
    }
}