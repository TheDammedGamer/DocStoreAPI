﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocStoreAPI.Repositories;
using DocStoreAPI.Models;
using Microsoft.Extensions.Logging;

namespace DocStoreAPI.Controllers
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
        // GET: api/DocumentMetadata/HR
        [HttpGet("{buisnessArea}")]
        public IActionResult ListByBuisnessArea(string buisnessArea)
        {
            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Return, buisnessArea))
                return _securityRepository.GateUnathorised(HttpContext, AccessLogAction.DocumentMetadataSearched, "Metadata", buisnessArea);

            List<MetadataEntity> metaItems = _metadataRepository.ListByBuisnessArea(buisnessArea, false, false).ToList();

            if (metaItems.Count == 0)
                return _securityRepository.GateNotFound(HttpContext, AccessLogAction.DocumentMetadataSearched, "Metadata", buisnessArea);

            _logger.Log(LogLevel.Information, "Getting DocumentMetadata Within BuisnessArea {0} for User {1}", buisnessArea, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentMetadataSearched, buisnessArea, "Metadata", true);

            return Ok(metaItems);
        }

        //Actual Result is MetadataEntity
        // GET: api/DocumentMetadata/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            MetadataEntity item = _metadataRepository.GetById(id, false, false);

            if (item == null)
                return _securityRepository.GateNotFound(HttpContext, AccessLogAction.DocumentMetadataRead, "Metadata", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Return, item.BuisnessArea))
                return _securityRepository.GateUnathorised(HttpContext, AccessLogAction.DocumentMetadataRead, "Metadata", id.ToString());

            _logger.Log(LogLevel.Information, "DocumentMetadata {0} Returned For {1}", item.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentMetadataRead, id, "Metadata", true);

            return Ok(item);
        }

        // POST: api/DocumentMetadata
        [HttpPost]
        public IActionResult Post([FromBody] MetadataEntity value)
        {
            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Create ,value.BuisnessArea))
                return _securityRepository.GateUnathorised(HttpContext, AccessLogAction.DocumentMetadataCreate, "Metadata", "N/A");

            var valueWId =_metadataRepository.Add(value);

            _logger.Log(LogLevel.Information, "DocumentMetadata {0} Created By {1}", valueWId.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentMetadataCreate, valueWId.Id, "Metadata", true);

            return Ok(valueWId);
        }

        // PUT: api/DocumentMetadata/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MetadataEntity value)
        {
            MetadataEntity origItem = _metadataRepository.GetById(id, false, false);

            if (origItem == null)
                return _securityRepository.GateNotFound(HttpContext, AccessLogAction.DocumentMetadataUpdate, "Metadata", id.ToString());

            //Check if user has access to both BuisnessAreas
            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Update, origItem.BuisnessArea, value.BuisnessArea))
                return _securityRepository.GateUnathorised(HttpContext, AccessLogAction.DocumentMetadataUpdate, "Metadata", id.ToString());

            _metadataRepository.Edit(value);

            _logger.Log(LogLevel.Information, "DocumentMetadata {0} Edited By {1}", value.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentMetadataUpdate, id, "Metadata", true);

            var meta = _metadataRepository.GetById(id, false, false);

            return Ok(meta);
        }

        // DELETE: api/DocumentMetadata/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            MetadataEntity origItem = _metadataRepository.GetById(id, true, true);

            if (origItem == null)
                return _securityRepository.GateNotFound(HttpContext, AccessLogAction.DocumentDelete, "Metadata", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Delete, origItem.BuisnessArea))
                return _securityRepository.GateUnathorised(HttpContext, AccessLogAction.DocumentDelete, "Metadata", id.ToString());

            foreach (var oldVer in origItem.Versions)
            {
                await _documentRepository.DeleteDocumentVersionAsync(oldVer);
                _logger.Log(LogLevel.Information, "Document File:'{0}' Deleted By {1}", oldVer.GetServerFileName(), HttpContext.User.Identity.Name);
            }

            await _documentRepository.DeleteDocumentAsync(origItem);

            _logger.Log(LogLevel.Information, "Document File:'{0}' Deleted By {1}", origItem.GetServerFileName(), HttpContext.User.Identity.Name);

            _metadataRepository.Delete(origItem);

            _logger.Log(LogLevel.Information, "Document {0} Deleted By {1}", origItem.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentDelete, id, "Document", true);

            return Ok();
        }

        // DELETE: api/DocumentMetadata/5/Archive
        [HttpDelete("{id}/Archive")]
        public IActionResult Archive(int id)
        {
            MetadataEntity origItem = _metadataRepository.GetById(id, false, false);

            if (origItem == null)
                return _securityRepository.GateNotFound(HttpContext, AccessLogAction.DocumentArchive, "Metadata", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Archive, origItem.BuisnessArea))
                return _securityRepository.GateUnathorised(HttpContext, AccessLogAction.DocumentArchive, "Metadata", id.ToString());

            origItem.Archive.Archive(HttpContext.User.Identity.Name);

            _logger.Log(LogLevel.Information, "Document {0} Archived By {1}", origItem.Id, HttpContext.User.Identity.Name);
            _securityRepository.LogUserAction(HttpContext.User.Identity.Name, AccessLogAction.DocumentArchive, id, "Document", true);

            _metadataRepository.Edit(origItem);

            return Ok();
        }
    }
}
