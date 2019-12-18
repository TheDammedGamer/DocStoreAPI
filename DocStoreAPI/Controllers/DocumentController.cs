using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocStore.Server.Repositories;
using DocStore.Server.Models;
using DocStore.Server.Extensions;
using DocStore.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace DocStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly MetadataRepository _metadataRepository;
        private readonly DocumentRepository _documentRepository;
        private readonly SecurityRepository _securityRepository;

        public DocumentController(ILogger<DocumentController> logger, MetadataRepository metadata, DocumentRepository documentRepository, SecurityRepository securityRepository)
        {
            _logger = logger;
            _metadataRepository = metadata;
            _documentRepository = documentRepository;
            _securityRepository = securityRepository;
        }

        // GET: api/Document/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            var currentUser = HttpContext.User.Identity.Name;

            var meta = _metadataRepository.GetById(id);

            if (meta == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentRead, "Document", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Return, meta.BuisnessArea))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentRead, "Document", id.ToString());

            var doc = await _documentRepository.GetDocumentAsync(meta);

            _logger.Log(LogLevel.Debug, "Downloading Document {0} for User {1}", meta.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentRead, id, "Document", true);

            return Ok(doc);
        }

        // POST: api/Document/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id, [FromBody] Stream value)
        {
            var currentUser = HttpContext.User.Identity.Name;

            //Posting a New document after posting the metadata
            var meta = _metadataRepository.GetById(id, true, false);

            if (meta == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentCreate, "Document", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Create, meta.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentCreate, "Document", id.ToString());

            await _documentRepository.SetDocumentAsync(meta, value);

            _logger.Log(LogLevel.Debug, "Uploaded New Document {0} for User {1}", meta.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentCreate, id, "Document", true);

            return Ok();
        }

        // PUT: api/Document/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Stream value)
        {
            var currentUser = HttpContext.User.Identity.Name;

            //uploading a new version of the document
            var meta = _metadataRepository.GetById(id, true, false);

            if (meta == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentUpdate, "Document", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Update, meta.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentUpdate, "Document", id.ToString());

            if (meta.Locked.Is && meta.Locked.By != currentUser)
                return _securityRepository.GateDocumentLockedByAnotherUser(currentUser, "Document", id.ToString());

            meta.Versions.Add(DocumentVersionEntity.FromMetadata(meta));

            meta.Version++;

            await _documentRepository.SetDocumentAsync(meta, value);

            _logger.Log(LogLevel.Debug, "Uploaded New Document Version {0} for User {1}", meta.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentUpdate, id, "Document", true);

            _metadataRepository.Edit(meta);
            _metadataRepository.SaveChanges();

            return Ok();
        }

        // DELETE: api/Document/5/Version/1
        [HttpDelete("{id}/Version/{verId}")]
        public async Task<IActionResult> Delete(int id, int verId)
        {
            var currentUser = HttpContext.User.Identity.Name;

            //Delete a Document Version, Deleting a complete document is in the Document Metadata Controller
            var meta = _metadataRepository.GetById(id, true, true);

            var ids = string.Format("{0}:{1}", id.ToString(), verId.ToString());

            if (meta == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentVersionDelete, "DocumentVersion", ids);

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Delete, meta.BuisnessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.DocumentVersionDelete, "DocumentVersion", ids);

            var oldVer = meta.Versions.FirstOrDefault(ver => ver.Version == verId);

            if (oldVer.Name == null)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.DocumentVersionDelete, "DocumentVersion", ids);

            await _documentRepository.DeleteDocumentVersionAsync(oldVer);

            _logger.Log(LogLevel.Debug, "Document File:'{0}' Deleted By {1}", oldVer.GetServerFileName(), HttpContext.User.Identity.Name);

            _logger.Log(LogLevel.Debug, "Deleteing Document {0} Version {1} for User {2}", meta.Id, oldVer.Version, HttpContext.User.Identity.Name);

            _metadataRepository.DeleteVersion(oldVer);
            _metadataRepository.SaveChanges();

            _securityRepository.LogUserAction(currentUser, AccessLogAction.DocumentVersionDelete, id, "DocumentVersion", true);

            return Ok();
        }
    }
}
