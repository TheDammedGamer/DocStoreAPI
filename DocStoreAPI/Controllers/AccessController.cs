using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocStoreAPI.Models;
using DocStoreAPI.Repositories;

namespace DocStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {

        private readonly AccessRepository _accessRepository;
        private readonly SecurityRepository _securityRepository;
        public AccessController(AccessRepository groupRepository, SecurityRepository securityRepository)
        {
            _accessRepository = groupRepository;
            _securityRepository = securityRepository;
        }
        // GET: api/Access
        [HttpGet]
        public IActionResult Get()
        {
            string currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACEList, "AccessControlEntity", string.Empty);

            var entities = _accessRepository.List();

            return Ok(entities);
        }

        // GET: api/Access/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            string currentUser = HttpContext.User.Identity.Name;

            var result = _accessRepository.GetById(id);

            if (String.IsNullOrWhiteSpace(result.BusinessArea) || String.IsNullOrWhiteSpace(result.Group))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.ACEReturn, "AccessControlEntity", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Supervisor, result.BusinessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACEReturn, "AccessControlEntity", id.ToString());

            return Ok(result);
        }

        // POST: api/Access
        [HttpPost]
        public IActionResult Post([FromBody] AccessControlEntity value)
        {
            string currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Supervisor, value.BusinessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACECreate, "AccessControlEntity", "NA");

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
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.ACEUpdate, "AccessControlEntity", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Supervisor, result.BusinessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACEUpdate, "AccessControlEntity", id.ToString());

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
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.ACEDelete, "AccessControlEntity", id.ToString());

            if (!_securityRepository.UserIsAuthorisedByBuisnessAreas(HttpContext, AuthActions.Supervisor, entity.BusinessArea))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.ACEDelete, "AccessControlEntity", entity.Id.ToString());
            _accessRepository.DeleteById(id);
            _accessRepository.SaveChanges();

            return Ok();
        }
    }
}
