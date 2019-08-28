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
            var query = _context.MetadataEntities.FirstOrDefault(m => m.Id == id);

            return query;
        }

        public IEnumerable<MetadataEntity> List(bool includeArchive = false)
        {
            if (includeArchive)
            {
                return _context.MetadataEntities
                .Include(m => m.BuisnessMetadata)
                .AsEnumerable();
            }
            else
            {
                return _context.MetadataEntities
                .Where(meta => !meta.Archive.Is)
                .Include(m => m.BuisnessMetadata)
                .AsEnumerable();
            }
        }
        public List<MetadataEntity> ListByBuisnessArea(string buisnessArea, int page, int perPage, out int totalPages, bool includeArchive = false, bool includeOldVersions = false, bool getAll = false)
        {
            if (getAll)
            {
                totalPages = 1;
                return ListByBuisnessAreaAll(buisnessArea, includeArchive, includeOldVersions);
            }
            else
            {
                return ListByBuisnessAreaPaged(buisnessArea, page, perPage, out totalPages, includeArchive, includeOldVersions)
            }
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

        public IEnumerable<MetadataEntity> SearchByCustomMetadataKey(string key, string value)
        {
            return _context.MetadataEntities.Where(meta =>
                meta.BuisnessMetadata
                    .Any(cm => cm.Key == key && cm.Value == value) && !meta.Archive.Is)
                .Include(m => m.BuisnessMetadata)
                .AsEnumerable();
        }

        //TODO: Sort this shit out to use BuisnessMetadata instead of CustomMetadata
        public IEnumerable<MetadataEntity> SearchByCustomMetadataKeys(List<string> keys, List<string> values)
        {
            if (keys.Count() != values.Count())
                throw new ArgumentException("arguments are not equal");

            IQueryable<BuisnessMetadata> query1 = _context.BuisnessMetadata.Where(bm => string.IsNullOrWhiteSpace(bm.Key));

            List<BuisnessMetadata> query1Results = new List<BuisnessMetadata>();
            List<BuisnessMetadata> query2Results = new List<BuisnessMetadata>();

            query1Results = query1.Where(bm => keys.Contains(bm.Key)).ToList();

            for (int i = 0; i < keys.Count(); i++)
            {
                query2Results.AddRange(query1Results.Where(bm => bm.Key == keys[i] && bm.Value == values[i]).ToList());
            }

            List<MetadataEntity> metaResults = new List<MetadataEntity>();

            foreach (var item in query2Results)
            {
                var metaItem = _context.MetadataEntities.FirstOrDefault(me => me.Id == item.DocumentId);
                if (!string.IsNullOrEmpty(metaItem.Name))
                    metaResults.Add(metaItem);
            }

            return metaResults;
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

        public List<MetadataEntity> SearchByBuisnessMetadata(BuisnessMetadataSearch search, bool incArchive)
        {
            List<MetadataEntity> results = new List<MetadataEntity>();
            List<int> documentIds = new List<int>();

            foreach (var searchItem in search.Search)
            {
                switch (searchItem.Type)
                {
                    case "Equality":
                        {
                            var item = (SearchEquality)searchItem;

                            List<int> docIds = new List<int>();

                            var query = _context.MetadataEntities.Where(qe => qe.BuisnessArea == search.BusinessArea);

                            if (item.Invert)
                            {
                                //Not Equal Search
                                var bms = _context.BuisnessMetadata.Where(bm => bm.Key == item.Key && bm.Value != item.Value);

                                foreach (var buisnessMetadata in bms)
                                    docIds.Add(buisnessMetadata.DocumentId);
                            }
                            else
                            {
                                // Equals search
                                var bms = _context.BuisnessMetadata.Where(bm => bm.Key == item.Key && bm.Value == item.Value);

                                foreach (var buisnessMetadata in bms)
                                    docIds.Add(buisnessMetadata.DocumentId);
                            }

                            SearchByBuisnessMetadataResults(docIds, search.Joiner, search.BusinessArea, ref results);

                            break;
                        }
                    case "Present":
                        {
                            var item = (SearchPresent)searchItem;

                            List<int> docIds = new List<int>();

                            if (item.Invert)
                            {
                                //Is Not Present
                                var bms = _context.BuisnessMetadata.Where(bm => bm.Key == item.Key && string.IsNullOrEmpty(bm.Value));

                                foreach (var buisnessMetadata in bms)
                                    docIds.Add(buisnessMetadata.DocumentId);
                            }
                            else
                            {
                                //Is Present
                                var bms = _context.BuisnessMetadata.Where(bm => bm.Key == item.Key && !string.IsNullOrEmpty(bm.Value));

                                foreach (var buisnessMetadata in bms)
                                    docIds.Add(buisnessMetadata.DocumentId);
                            }

                            SearchByBuisnessMetadataResults(docIds, search.Joiner, search.BusinessArea, ref results);
                            break;
                        }
                    case "AllKeys":
                        {
                            var item = (SearchAllKeys)searchItem;

                            List<int> docIds = new List<int>();

                            var bms =_context.BuisnessMetadata.Where(bm => bm.Value == item.Value).ToList();

                            foreach (var buisnessMetadata in bms)
                                docIds.Add(buisnessMetadata.DocumentId);


                            SearchByBuisnessMetadataResults(docIds, search.Joiner, search.BusinessArea, ref results);

                            break;
                        }
                    default:

                        break;
                }
            }

            return results.Distinct().Where(me => me.Archive.Is == incArchive).ToList()
        }
        private void SearchByBuisnessMetadataResults(List<int> docIds, SearchJoin joinType, string BuisnessArea, ref List<MetadataEntity> currentResults)
        {
            if (joinType == SearchJoin.And)
            {
                if (currentResults.Count > 0)
                    currentResults = currentResults.Where(res => docIds.Contains(res.Id)).ToList(); //filter current results
                else
                    currentResults.AddRange(_context.MetadataEntities.Where(meta => meta.BuisnessArea == BuisnessArea  && docIds.Contains(meta.Id)).ToList());
            }
            else
                currentResults.AddRange(_context.MetadataEntities.Where(meta => meta.BuisnessArea == BuisnessArea && docIds.Contains(meta.Id)).ToList());
        }
    }
}
