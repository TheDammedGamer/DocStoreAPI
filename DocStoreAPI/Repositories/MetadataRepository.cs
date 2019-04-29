using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocStoreAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DocStoreAPI.Repositories
{
    public class MetadataRepository
    {

        private readonly DocStoreContext _context;
        private readonly ILogger _logger;

        public MetadataRepository(ILogger<MetadataRepository> logger, DocStoreContext docStoreContext)
        {
            _logger = logger;
            _context = docStoreContext;
        }

        public MetadataEntity Add(MetadataEntity entity)
        {
            _context.MetadataEntities.Add(entity);
            
            _context.SaveChanges();

            return entity; //Reutrns with the ID Included
        }

        public void Delete(MetadataEntity entity)
        {
            _context.DocumentVersions.RemoveRange(entity.Versions);
            
            _context.MetadataEntities.Remove(entity);

            _context.SaveChanges();
        }

        public void DeleteVersion(DocumentVersionEntity entity)
        {
            _context.DocumentVersions.Remove(entity);

            _context.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var entity = GetById(id, true, true);

            this.Delete(entity);

            entity = null;
        }

        public void Edit(MetadataEntity entity)
        {
            // Get the item so we don't just mark the entire object as changed
            GetById(entity.Id, true, false);
            _context.MetadataEntities.Update(entity);
            _context.SaveChanges();
        }

        public MetadataEntity GetById(int id)
        {
            return GetById(id, false, false);
        }
        public MetadataEntity GetById(int id, bool includeArchive, bool includeOldVersions)
        {
            var entities = _context.MetadataEntities
                .Where(meta => meta.Id == id);

            if (!includeArchive)
                entities = entities.Where(m => !m.Archive.Is);

            entities = entities.Include(m => m.CustomMetadata);
            if (includeOldVersions)
                entities = entities.Include(m => m.Versions);

            return entities.SingleOrDefault();
        }

        public IEnumerable<MetadataEntity> List()
        {
            return List(false);
        }

        public IEnumerable<MetadataEntity> List(bool includeArchive)
        {
            if (includeArchive)
            {
                return _context.MetadataEntities
                .Include(m => m.CustomMetadata)
                .AsEnumerable();
            }
            else
            {
                return _context.MetadataEntities
                .Where(meta => !meta.Archive.Is)
                .Include(m => m.CustomMetadata)
                .AsEnumerable();
            }
        }

        public IEnumerable<MetadataEntity> ListByBuisnessArea(string buisnessArea)
        {
            return ListByBuisnessArea(buisnessArea, false, false);
        }

        public IEnumerable<MetadataEntity> ListByBuisnessArea(string buisnessArea, bool includeArchive, bool includeOldVersions)
        {
            var entities = _context.MetadataEntities
                .Where(meta => meta.BuisnessArea == buisnessArea);

            if (!includeArchive)
                entities = entities.Where(m => !m.Archive.Is);

            entities = entities.Include(m => m.CustomMetadata);
            if (includeOldVersions)
                entities = entities.Include(m => m.Versions);

            return entities.AsEnumerable();
        }

        public IEnumerable<MetadataEntity> List(Expression<Func<MetadataEntity, bool>> predicate)
        {
            return List(predicate, false, false);
        }

        public IEnumerable<MetadataEntity> List(Expression<Func<MetadataEntity, bool>> predicate, bool includeArchive, bool includeOldVersions)
        {
            var entities = _context.MetadataEntities
                .Where(predicate);
            if (!includeArchive)
                entities = entities.Where(m => !m.Archive.Is);
            
            entities = entities.Include(m => m.CustomMetadata);
            if (includeOldVersions)
                entities = entities.Include(m => m.Versions);

            return entities.AsEnumerable();
        }

        public IEnumerable<MetadataEntity> SearchByCustomMetadataKey(string key, string value)
        {
            return _context.MetadataEntities.Where(meta =>
                meta.CustomMetadata
                    .Any(cm => cm.Key == key && cm.Value == value) && !meta.Archive.Is)
                .Include(m => m.CustomMetadata)
                .AsEnumerable();
        }

        public IEnumerable<MetadataEntity> SearchByCustomMetadataKeys(string[] keys, string[] values)
        {
            if (keys.Count() != values.Count())
                throw new ArgumentException("arguments are not equal");
            
            return _context.MetadataEntities.Where(meta =>
                meta.CustomMetadata
                    .Any(cm => keys.Contains(cm.Key) && values.Contains(cm.Value)) && !meta.Archive.Is)
                .Include(m => m.CustomMetadata)
                .AsEnumerable();
        }
    }
}
