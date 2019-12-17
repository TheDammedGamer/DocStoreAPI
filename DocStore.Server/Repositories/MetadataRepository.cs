using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
//using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using DocStore.Server.Models;
using DocStore.Server.Extensions;
using DocStore.Shared.Models;
using DocStore.Shared.Models.Search;

namespace DocStore.Server.Repositories
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
        }

        public void Delete(MetadataEntity entity)
        {
            _context.DocumentVersions.RemoveRange(entity.Versions);

            _context.MetadataEntities.Remove(entity);
        }

        public void DeleteVersion(DocumentVersionEntity entity)
        {
            _context.DocumentVersions.Remove(entity);
        }

        public void DeleteById(int id)
        {
            var entity = GetById(id, true, true);

            Delete(entity);
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
            var query = _context.MetadataEntities.FirstOrDefault(m => m.Id == id && m.Archive.Is == includeArchive);

            return query;
        }

        public IEnumerable<MetadataEntity> List(bool includeArchive = false)
        {
            if (includeArchive)
                return _context.MetadataEntities.Include(m => m.BuisnessMetadata).AsEnumerable();
            else
                return _context.MetadataEntities.Where(meta => !meta.Archive.Is).Include(m => m.BuisnessMetadata).AsEnumerable();
            
        }
        public List<MetadataEntity> ListByBuisnessArea(string buisnessArea, int page, int perPage, out int totalPages, bool includeArchive = false, bool includeOldVersions = false, bool getAll = false)
        {
            if (getAll)
            {
                totalPages = 1;
                return ListByBuisnessAreaAll(buisnessArea, includeArchive, includeOldVersions);
            }
            else
                return ListByBuisnessAreaPaged(buisnessArea, page, perPage, out totalPages, includeArchive, includeOldVersions);
            
        }

        private List<MetadataEntity> ListByBuisnessAreaAll(string buisnessArea, bool includeArchive, bool includeOldVersions)
        {
            var entities = _context.MetadataEntities
                .Where(meta => meta.BuisnessArea == buisnessArea);

            if (!includeArchive)
                entities = entities.Where(m => !m.Archive.Is);

            entities = entities.Include(m => m.BuisnessMetadata);
            if (includeOldVersions)
                entities = entities.Include(m => m.Versions);

            return entities.ToList();
        }

        private List<MetadataEntity> ListByBuisnessAreaPaged(string buisnessArea, int page, int perPage, out int totalPages, bool includeArchive, bool includeOldVersions)
        {
            var entities = _context.MetadataEntities
                .Where(meta => meta.BuisnessArea == buisnessArea);

            if (!includeArchive)
                entities = entities.Where(m => !m.Archive.Is);

            entities = entities.Include(m => m.BuisnessMetadata);
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

            entities = entities.Include(m => m.BuisnessMetadata);
            if (includeOldVersions)
                entities = entities.Include(m => m.Versions);

            return entities.AsEnumerable();
        }

        public List<MetadataEntity> SearchByMetadataSearch(MetadataSearch search, bool incArchive)
        {
            var searchContext = _context.MetadataEntities.AsQueryable().Where(gg => gg.BuisnessArea == search.BusinessArea && gg.Archive.Is == incArchive);

            List<MetadataEntity> results = new List<MetadataEntity>();

            foreach (var item in search.Search)
            {
                switch (item.Type)
                {
                    case "AllKeys":
                        {
                            var keys = (SearchAllKeys)item;
                            var tmp = searchContext.Where(gg => gg.Metadata.ContainsValue(keys.Value)).ToList();
                            if (search.Joiner == SearchJoin.And)
                                results.AddRange(tmp);
                            else
                            {
                                if (results.Count >= 1)
                                    results.AddRange(tmp);
                                else
                                    results.RemoveAll(it => tmp.Contains(it));
                            }
                            break;
                        }
                    case "Equality":
                        {
                            var keys = (SearchEquality)item;
                            List<MetadataEntity> entities = new List<MetadataEntity>();
                            if (keys.Invert)
                                entities.AddRange(searchContext.Where(gg => gg.Metadata[keys.Key] != keys.Value).ToList());
                            else
                                entities.AddRange(searchContext.Where(gg => gg.Metadata[keys.Key] == keys.Value).ToList());

                            if (search.Joiner == SearchJoin.And)
                                results.AddRange(entities);
                            else
                            {
                                if (results.Count >= 1)
                                    results.AddRange(entities);
                                else
                                    results.RemoveAll(it => entities.Contains(it));
                            }
                            break;
                        }
                    case "Present":
                        {
                            var keys = (SearchPresent)item;
                            List<MetadataEntity> entities = new List<MetadataEntity>();
                            if (keys.Invert)
                                entities.AddRange(searchContext.Where(gg => gg.Metadata.ContainsKey(keys.Key) == false).ToList());
                            else
                                entities.AddRange(searchContext.Where(gg => gg.Metadata.ContainsKey(keys.Key)).ToList());

                            if (search.Joiner == SearchJoin.And)
                                results.AddRange(entities);
                            else
                            {
                                if (results.Count >= 1)
                                    results.AddRange(entities);
                                else
                                    results.RemoveAll(it => entities.Contains(it));
                            }
                            break;
                        }
                    default:
                        throw new Exception("Unable to read Input SearchType");
                }
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

        public void AddNew(ref MetadataEntity entity, string user)
        {
            var timenow = DateTime.UtcNow;

            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                switch (property.Name)
                {
                    case "Versions":
                        entity.Versions = new List<DocumentVersionEntity>();
                        break;
                    case "LastViewed":
                        entity.LastViewed = timenow;
                        break;
                    case "LastUpdate":
                        entity.LastUpdate.ServerUpdate(user, timenow);
                        break;
                    case "Created":
                        entity.Created.ServerUpdate(user, timenow);
                        break;
                    case "Archive":
                        entity.Archive = new ArchiveState();
                        break;
                    case "MD5Hash":
                        entity.MD5Hash = string.Empty;
                        break;
                    case "Version":
                        entity.Version = 1;
                        break;
                    case "Name":
                        if (string.IsNullOrWhiteSpace(entity.Name))
                            throw new ArgumentNullException("Name");
                        break;
                    case "StorName":
                        if (string.IsNullOrWhiteSpace(entity.StorName))
                            throw new ArgumentNullException("StorName");
                        break;
                    case "Extension":
                        if (string.IsNullOrWhiteSpace(entity.Extension))
                            throw new ArgumentNullException("Extension");
                        break;
                    case "BuisnessArea":
                        if (string.IsNullOrWhiteSpace(entity.BuisnessArea))
                            throw new ArgumentNullException("BuisnessArea");
                        break;
                    case "Id":
                        if (property != null)
                            throw new ArgumentException("Id can not be set on client", "Id");
                        break;
                    default:
                        //not too fussed about the rest of the properties as theese are designed to be set by user.
                        break;
                }
            }
            _context.MetadataEntities.Add(entity);
        }

        public MetadataEntity UserEdit(ref MetadataEntity entityOriginal, MetadataEntity entity, HttpContext context)
        {
            bool rename = false;
            bool changed = false;

            foreach (PropertyInfo property in entityOriginal.GetType().GetProperties())
            {
                var origValue = property.GetValue(entityOriginal);
                var newValue = property.GetValue(entity);

                if (origValue != newValue)
                {
                    switch (property.Name)
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
                            entityOriginal.BuisnessArea = entity.BuisnessArea;
                            break;
                        case "BuisnessMetadata":
                            changed = true;
                            entityOriginal.BuisnessMetadata = entity.BuisnessMetadata;
                            break;
                        default:
                            break;
                            //throw new Exception("Unable to Change this Property via this API Call");
                    }
                }
            }

            if (changed)
            {
                entityOriginal.LastUpdate.Update(context.User.Identity.Name);
                Touch(ref entityOriginal);
            }

            if (rename)
            {
                _documentRepository.RenameFileAsync(entityOriginal, entityOriginal).RunSynchronously();
            }

            return entityOriginal;
        }
    }
}
