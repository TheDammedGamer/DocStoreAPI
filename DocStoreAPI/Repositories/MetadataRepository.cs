using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocStoreAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace DocStoreAPI.Repositories
{
    public class MetadataRepository : BaseRepository
    {
        private readonly DocStoreContext _context;
        private readonly DocumentRepository _documentRepository;
        private readonly SecurityRepository _securityRepository;
        private readonly ILogger _logger;

        public MetadataRepository(ILogger<MetadataRepository> logger, DocStoreContext docStoreContext, DocumentRepository repository, SecurityRepository securityRepository)
        {
            _logger = logger;
            _context = docStoreContext;
            _documentRepository = repository;
            _securityRepository = securityRepository;
        }

        public void Add(ref MetadataEntity entity)
        {
            _context.MetadataEntities.Add(entity);

            this.SaveChanges();

            //return entity;
            //don't need to return the entity as it will pass the pk back to the original entity
        }

        public void Delete(MetadataEntity entity)
        {
            _context.DocumentVersions.RemoveRange(entity.Versions);

            _context.MetadataEntities.Remove(entity);


            this.SaveChanges();
        }

        public void DeleteVersion(DocumentVersionEntity entity)
        {
            _context.DocumentVersions.Remove(entity);

            this.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var entity = GetById(id, true, true);

            this.Delete(entity);
        }

        public void Edit(MetadataEntity entity)
        {
            // Get the item so we don't just mark the entire object as changed
            GetById(entity.Id, true, false);
            _context.MetadataEntities.Update(entity);
        }

        public void Touch(ref MetadataEntity entity)
        {
            entity.LastViewed = DateTime.UtcNow;
        }

        public MetadataEntity GetById(int id, bool includeArchive = false, bool includeOldVersions = false)
        {
            var query = _context.MetadataEntities.FirstOrDefault(m => m.Id == id);

            return query;
        }

        public IEnumerable<MetadataEntity> List(bool includeArchive = false)
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

        public IEnumerable<MetadataEntity> ListByBuisnessArea(string buisnessArea, bool includeArchive = false, bool includeOldVersions = false)
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

        public List<MetadataEntity> ListByBuisnessAreaPaged(string buisnessArea, int page, int perPage, out int totalPages, bool includeArchive = false, bool includeOldVersions = false)
        {
            var entities = _context.MetadataEntities
                .Where(meta => meta.BuisnessArea == buisnessArea);

            if (!includeArchive)
                entities = entities.Where(m => !m.Archive.Is);

            entities = entities.Include(m => m.CustomMetadata);
            if (includeOldVersions)
                entities = entities.Include(m => m.Versions);

            int totalRecords = entities.Count();

            totalPages = 0;
            if (totalRecords > 0)
                totalPages = (((totalRecords - 1) / perPage) + 1);

            if (page == 0)
                entities = entities.Take(perPage);
            else
                entities = entities.Skip(page * perPage).Take(perPage);

            return entities.ToList();
        }

        public IEnumerable<MetadataEntity> List(Expression<Func<MetadataEntity, bool>> predicate, bool includeArchive = false, bool includeOldVersions = false)
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

        public IEnumerable<MetadataEntity> SearchByCustomMetadataKeys(List<string> keys, List<string> values)
        {
            if (keys.Count() != values.Count())
                throw new ArgumentException("arguments are not equal");

            IQueryable<CustomMetadataEntity> query = _context.CustomMetadataEntities.Where(cm => cm.Document != null);

            query = query.Include(cm => cm.Document).ThenInclude(me => me.CustomMetadata);

            query = query.Include(cm => cm.Document).ThenInclude(me => me.Archive);

            query = query.Where(cm => !cm.Document.Archive.Is);

            var res = query.Where(cm => keys.Contains(cm.Key)).ToList();

            List<CustomMetadataEntity> queryResults = new List<CustomMetadataEntity>();

            for (int i = 0; i < keys.Count(); i++)
            {
                queryResults.AddRange(res.Where(cm => cm.Key == keys[i] && cm.Value == values[i]).ToList());
            }

            List<MetadataEntity> results = new List<MetadataEntity>();

            foreach (var item in queryResults)
            {
                results.Add(item.Document);
            }

            return results;
        }

        public bool TryLockDocument(ref MetadataEntity entity, HttpContext context, TimeSpan? duration = null)
        {
            var realDuration = duration ?? TimeSpan.FromHours(3);

            var success = entity.Locked.LockDocument(context.User.Identity.Name, realDuration);

            return success;
        }

        public bool TryUnlockDocument(ref MetadataEntity entity, HttpContext context)
        {
            var success = entity.Locked.UnLockDocument(context.User.Identity.Name);

            return success;
        }

        public MetadataEntity UserEdit(ref MetadataEntity entityOriginal, MetadataEntity entity, HttpContext context)
        {
            bool rename = false;
            bool changed = false;
            bool baChange = false;

            foreach (PropertyInfo property in entityOriginal.GetType().GetProperties())
            {
                var origValue = property.GetValue(entityOriginal);
                var newValue = property.GetValue(entity);

                if (origValue != newValue)
                {
                    

                    switch(property.Name)
                    {
                        case "Name":
                            changed = true;
                            rename = true;
                            entityOriginal.Name = entity.Name;
                            break;
                        case "Version":
                            changed = true;
                            rename = true;
                            entityOriginal.Version = entity.Version;
                            break;
                        case "Extension":
                            changed = true;
                            rename = true;
                            entityOriginal.Extension = entity.Extension;
                            break;
                        case "BuisnessArea":
                            changed = true;
                            baChange = true;
                            entityOriginal.BuisnessArea = entity.BuisnessArea;
                            break;
                        case "CustomMetadata":
                            changed = true;
                            entityOriginal.CustomMetadata = entity.CustomMetadata;
                            break;
                        default:
                            throw new Exception("Unable to Change this Property via this API Call");
                    }
                }
            }

            if (changed)
            {
                entityOriginal.LastUpdate.Update(context.User.Identity.Name);
                Touch(ref entityOriginal);
            }
            if (baChange)
            {
                var isAuthed = _securityRepository.UserIsAuthorisedByBuisnessAreas(context, AuthActions.Update, entityOriginal.BuisnessArea);

                if (!isAuthed)
                {
                    _context.RejectModifications();
                    throw new Exception("User does not Have access to chnaged the document's BuisnessArea");
                }
            }

            if (rename)
            {
                _documentRepository.RenameFileAsync(entityOriginal, entityOriginal).RunSynchronously();
            }

            return entityOriginal;
        }

    }
}
