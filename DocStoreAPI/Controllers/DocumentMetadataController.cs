using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocStore.Server.Repositories;
using DocStore.Server.Models;
using DocStore.Server.Extensions;
using DocStore.Shared.Models;
using DocStore.Shared.Models.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace DocStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentMetadataController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly MetadataRepository _metadataRepository;
        private readonly DocumentRepository _documentRepository;
        private readonly SecurityRepository _securityRepository;

        public DocumentMetadataController(ILogger<DocumentMetadataController> logger, MetadataRepository metadata, DocumentRepository documentRepository, SecurityRepository securityRepository)
        {
            _metadataRepository = metadata;
            _logger = logger;
            _documentRepository = documentRepository;
            _securityRepository = securityRepository;
        }

        //Actual result is List<MetadataEntity>
        // GET: api/DocumentMetadata/HR?page=1&perPage=50
        [HttpGet("{buisnessArea}")]
        public IActionResult ListByBuisnessArea(string buisnessArea, [FromQuery]int page = 0, [FromQuery] int perPage = 25, [FromQuery] bool getAll = false)
        {
            string currentUser = HttpContext.User.Identity.Name;
            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Return, buisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentMetadataSearch, "Metadata", buisnessArea);

            int pageCount;
            List<MetadataEntity> metaItems = _metadataRepository.ListByBuisnessArea(buisnessArea, page, perPage, out pageCount, getAll: getAll);

            if (metaItems.Count == 0)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentMetadataSearch, "Metadata", buisnessArea);

            _logger.Log(LogLevel.Information, "Getting DocumentMetadata Within BuisnessArea {0} for User {1}", buisnessArea, currentUser);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentMetadataSearch, buisnessArea, "Metadata", true);

            HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("TotalPages", pageCount.ToString()));

            return Ok(metaItems);
        }

        //Actual Result is MetadataEntity
        // GET: api/DocumentMetadata/5?incArchive=false;incVersions=true
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id, [FromQuery] bool incVersions = false, [FromQuery] bool incArchive = false)
        {
            string currentUser = HttpContext.User.Identity.Name;
            MetadataEntity item = _metadataRepository.GetById(id, incVersions, incArchive);

            if (item == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentMetadataRead, "Metadata", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Return, item.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentMetadataRead, "Metadata", id.ToString());

            _logger.Log(LogLevel.Information, "DocumentMetadata {0} Returned For {1}", item.Id, currentUser);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentMetadataRead, id, "Metadata", true);

            _metadataRepository.Touch(ref item);
            _metadataRepository.SaveChanges();

            return Ok(item);
        }

        //Actual Result is List<MetadataEntity>
        // PUT: api/DocumentMetadata/Search?incArchive=true
        // PUT: api/DocumentMetadata/Search
        [HttpPut("/Search", Name = "Search")]
        public IActionResult Search([FromBody]MetadataSearch search, [FromQuery] bool incArchive = false)
        {
            string currentUser = HttpContext.User.Identity.Name;

            if (search == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentMetadataSearch, "Metadata", search.BusinessArea);

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Return, search.BusinessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentMetadataSearch, "Metadata", search.BusinessArea);

            var result = _metadataRepository.SearchByMetadataSearch(search, incArchive);

            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentMetadataSearch, search.BusinessArea, "Metadata", true);

            _securityRepository.LogUserSearch(currentUser, search);

            _securityRepository.SaveChanges();

            return Ok(result);
        }


        //Actual Result is MetadataEntity
        // PUT: api/DocumentMetadata/5/Lock
        [HttpPut("{id}/Lock", Name = "Lock")]
        public IActionResult Lock(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;
            MetadataEntity item = _metadataRepository.GetById(id);

            if (item == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentLocked, "Metadata", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Update, item.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentLocked, "Metadata", id.ToString());

            bool success =_metadataRepository.TryLockDocument(ref item, HttpContext);

            if (!success)
                return _securityRepository.GateCannotLock(currentUser, "Metadata", id.ToString());

            _metadataRepository.Touch(ref item);

            _logger.Log(LogLevel.Debug, "DocumentMetadata {0} Locked By {1}", item.Id, currentUser);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentLocked, item.Id.ToString(), "Metadata", true);

            _metadataRepository.SaveChanges();

            return Ok(success);
        }

        //Actual Result is MetadataEntity
        // PUT: api/DocumentMetadata/5/Unlock
        [HttpPut("{id}/Unlock", Name = "Unlock")]
        public IActionResult Unlock(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;
            MetadataEntity item = _metadataRepository.GetById(id, false, false);

            if (item == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentUnlocked, "Metadata", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Update, item.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentUnlocked, "Metadata", id.ToString());

            bool success = _metadataRepository.TryUnlockDocument(ref item, HttpContext);

            if (!success)
                return _securityRepository.GateCannotUnlock(currentUser, "Metadata", id.ToString());

            _metadataRepository.Touch(ref item);
            
            _logger.Log(LogLevel.Debug, "DocumentMetadata {0} Locked By {1}", item.Id, currentUser);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentUnlocked, item.Id.ToString(), "Metadata", true);

            _metadataRepository.SaveChanges();

            return Ok(success);
        }

        // POST: api/DocumentMetadata
        [HttpPost]
        public IActionResult Post([FromBody] MetadataEntity value)
        {
            string currentUser = HttpContext.User.Identity.Name;
            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Create ,value.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentMetadataCreate, "Metadata", "N/A");

            _metadataRepository.AddNew(ref value, currentUser);

            _logger.Log(LogLevel.Debug, "DocumentMetadata {0} Created By {1}", value.Id, currentUser);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentMetadataCreate, value.Id, "Metadata", true);

            _metadataRepository.SaveChanges();

            return Ok(value);
        }

        // PUT: api/DocumentMetadata/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MetadataEntity value)
        {
            string currentUser = HttpContext.User.Identity.Name;
            MetadataEntity origItem = _metadataRepository.GetById(id, false, false);

            if (origItem == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentMetadataUpdate, "Metadata", id.ToString());
            
            //Check if user has access to both BuisnessAreas Will work even if both are the same
            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Update, origItem.BuisnessArea, value.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentMetadataUpdate, "Metadata", id.ToString());

            _metadataRepository.UserEdit(ref origItem, value, HttpContext);

            _logger.Log(LogLevel.Debug, "DocumentMetadata {0} Edited By {1}", origItem.Id, currentUser);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentMetadataUpdate, origItem.Id.ToString(), "Metadata", true);

            _metadataRepository.SaveChanges();

            return Ok(origItem);
        }

        // DELETE: api/DocumentMetadata/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;

            MetadataEntity origItem = _metadataRepository.GetById(id, true, true);

            if (origItem == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentDelete, "Metadata", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Delete, origItem.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentDelete, "Metadata", id.ToString());

            foreach (var oldVer in origItem.Versions)
            {
                await _documentRepository.DeleteDocumentVersionAsync(oldVer);
                _logger.Log(LogLevel.Debug, "Document File:'{0}' Deleted By {1}", oldVer.GetServerFileName(), currentUser);
                _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentVersionDelete, oldVer.Id, "DocumentVersion", true);
            }

            await _documentRepository.DeleteDocumentAsync(origItem);

            _logger.Log(LogLevel.Debug, "Document File:'{0}' Deleted By {1}", origItem.GetServerFileName(), currentUser);

            _metadataRepository.Delete(origItem);

            _logger.Log(LogLevel.Debug, "Document {0} Deleted By {1}", origItem.Id, currentUser);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentDelete, id, "Document", true);

            _metadataRepository.SaveChanges();

            return Ok();
        }

        // DELETE: api/DocumentMetadata/5/Archive
        [HttpDelete("{id}/Archive")]
        public IActionResult Archive(int id)
        {
            MetadataEntity origItem = _metadataRepository.GetById(id, false, false);

            string currentUser = HttpContext.User.Identity.Name;

            if (origItem == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentArchive, "Metadata", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Archive, origItem.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentArchive, "Metadata", id.ToString());

            origItem.Archive.Archive(currentUser);

            _logger.Log(LogLevel.Debug, "Document {0} Archived By {1}", origItem.Id, currentUser);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentArchive, id, "Document", true);

            _metadataRepository.Edit(origItem);
            _metadataRepository.SaveChanges();

            return Ok();
        }
    }
}
