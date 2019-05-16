﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocStoreAPI.Models;
using DocStoreAPI.Repositories;
using Microsoft.Extensions.Primitives;

namespace DocStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {

        private readonly AccessRepository _accessRepository;
        private readonly SecurityRepository _securityRepository;
        private const string _object = "AccessControlEntity";
        public AccessController(AccessRepository groupRepository, SecurityRepository securityRepository)
        {
            _accessRepository = groupRepository;
            _securityRepository = securityRepository;
        }

        // GET: api/Access?page=1&perPage=30
        [HttpGet]
        public IActionResult Get([FromQuery]int page = 0, [FromQuery]int perPage = 25)
        {
            string currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACEList, _object, string.Empty);

            int pageCount = 0;

            var entities = _accessRepository.ListP(out pageCount, perPage, page);

            if (entities.Count() == 0)
                _securityRepository.GateNotFound(currentUser, AccessLogAction.ACEList, _object, string.Empty);

            HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("TotalPages", pageCount.ToString()));

            return Ok(entities);
        }

        // GET: api/Access/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;

            var result = _accessRepository.GetById(id);

            if (String.IsNullOrWhiteSpace(result.BusinessArea) || String.IsNullOrWhiteSpace(result.Group))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.ACEReturn, _object, id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Supervisor, result.BusinessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACEReturn, _object, id.ToString());

            return Ok(result);
        }

        // POST: api/Access
        [HttpPost]
        public IActionResult Post([FromBody] AccessControlEntity value)
        {
            string currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Supervisor, value.BusinessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACECreate, _object, string.Empty);

            _accessRepository.Add(value);
            _accessRepository.SaveChanges();

            return Ok(value);
        }

        // PUT: api/Access/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] AccessControlEntity value)
        {
            string currentUser = HttpContext.User.Identity.Name;

            var result = _accessRepository.GetById(id);

            if (String.IsNullOrWhiteSpace(result.BusinessArea) || String.IsNullOrWhiteSpace(result.Group))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.ACEUpdate, _object, id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Supervisor, result.BusinessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACEUpdate, _object, id.ToString());

            _accessRepository.Edit(value);
            _accessRepository.SaveChanges();

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;

            var entity = _accessRepository.GetById(id);

            if (String.IsNullOrWhiteSpace(entity.BusinessArea) || String.IsNullOrWhiteSpace(entity.Group))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.ACEDelete, _object, id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Supervisor, entity.BusinessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACEDelete, _object, entity.Id.ToString());

            _accessRepository.DeleteById(id);
            _accessRepository.SaveChanges();

            return Ok();
        }
    }
}
