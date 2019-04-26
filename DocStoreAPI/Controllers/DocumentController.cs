using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocStoreAPI.Repositories;
using DocStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace DocStoreAPI.Controllers
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
            var meta = _metadataRepository.GetById(id);

            if (!_securityRepository.UserIsAuthorisedByBuisnessArea(HttpContext, meta.BuisnessArea, "r"))
            {
                _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentRead, id, "Document", false);
                return Unauthorized();
            }

            var doc = await _documentRepository.GetDocumentAsync(meta);

            _logger.Log(LogLevel.Information, "Downloading Document {0} for User {1}", meta.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentRead, id, "Document", true);

            return Ok(doc);
        }

        // POST: api/Document/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id, [FromBody] Stream value)
        {
            //Posting a New document after posting the metadata
            var meta = _metadataRepository.GetById(id, true, false);

            if (!_securityRepository.UserIsAuthorisedByBuisnessArea(HttpContext, meta.BuisnessArea, "c"))
            {
                _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentCreate, id, "Document", false);
                return Unauthorized();
            }

            await _documentRepository.SetDocumentAsync(meta, value);

            _logger.Log(LogLevel.Information, "Uploaded New Document {0} for User {1}", meta.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentCreate, id, "Document", true);

            return Ok();
        }

        // PUT: api/Document/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Stream value)
        {
            //uploading a new version of the document
            var meta = _metadataRepository.GetById(id, true, false);

            if (!_securityRepository.UserIsAuthorisedByBuisnessArea(HttpContext, meta.BuisnessArea, "u"))
            {
                _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentUpdate, id, "Document", false);
                return Unauthorized();
            }
                


            meta.Versions.Add(DocumentVersionEntity.FromMetadata(meta));

            meta.Version++;

            await _documentRepository.SetDocumentAsync(meta, value);

            _logger.Log(LogLevel.Information, "Uploaded New Document Version {0} for User {1}", meta.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentUpdate, id, "Document", true);

            _metadataRepository.Edit(meta);

            return Ok();
        }

        // DELETE: api/Document/5/Version/1
        [HttpDelete("{id}/Version/{verId}")]
        public async Task<IActionResult> Delete(int id, int verId)
        {
            //Delete a Document Version, Deleteing a complete document is in the Document Metadata Controller
            var meta = _metadataRepository.GetById(id, true, true);

            if (!_securityRepository.UserIsAuthorisedByBuisnessArea(HttpContext, meta.BuisnessArea, "d"))
            {
                _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentVersionDelete, id, "DocumentVersion", false);
                return Unauthorized();
            }

            var oldVer = meta.Versions.First(ver => ver.Version == verId);

            await _documentRepository.DeleteDocumentVersionAsync(oldVer);

            _logger.Log(LogLevel.Information, "Document File:'{0}' Deleted By {1}", oldVer.GetServerFileName(), HttpContext.User.Identity.Name);

            _logger.Log(LogLevel.Information, "Deleteing Document {0} Version {1} for User {2}", meta.Id, oldVer.Version, HttpContext.User.Identity.Name);

            _metadataRepository.DeleteVersion(oldVer);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentVersionDelete, id, "DocumentVersion", true);

            return Ok();
        }
    }
}
