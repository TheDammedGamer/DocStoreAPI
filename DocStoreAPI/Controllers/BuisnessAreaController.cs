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
    public class BuisnessAreaController : ControllerBase
    {
        private readonly BuisnessAreaRepository _buisnessAreaRepository;
        private readonly SecurityRepository _securityRepository;
        private const string _objectType = "BuisnessArea";

        public BuisnessAreaController(BuisnessAreaRepository repository, SecurityRepository securityRepository)
        {
            _buisnessAreaRepository = repository;
            _securityRepository = securityRepository;
        }
        // GET: api/BuisnessArea
        [HttpGet]
        public IActionResult Get()
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BAList, _objectType, "NA");

            var buisnessAreas = _buisnessAreaRepository.List();

            return Ok(buisnessAreas);
        }

        // GET: api/BuisnessArea/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var currentUser = HttpContext.User.Identity.Name;

            var entity = _buisnessAreaRepository.GetById(id);

            if (String.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.BAReturn, _objectType, id.ToString());

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BAReturn, _objectType, id.ToString());

            return Ok(entity);
        }

        // POST: api/BuisnessArea
        [HttpPost]
        public IActionResult Post([FromBody] BuisnessAreaEntity value)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BACreate, _objectType, "NA");

            _buisnessAreaRepository.Add(value);
            _buisnessAreaRepository.SaveChanges();

            return Ok(value);
        }

        // PUT: api/BuisnessArea/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] BuisnessAreaEntity value)
        {
            var currentUser = HttpContext.User.Identity.Name;

            var entity = _buisnessAreaRepository.GetById(id);

            if (String.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.BAUpdate, _objectType, id.ToString());

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BAUpdate, _objectType, id.ToString());

            _buisnessAreaRepository.Edit(value);
            _buisnessAreaRepository.SaveChanges();

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var currentUser = HttpContext.User.Identity.Name;

            var entity = _buisnessAreaRepository.GetById(id);

            if (String.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.BADelete, _objectType, id.ToString());

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BADelete, _objectType, id.ToString());

            _buisnessAreaRepository.DeleteById(id);
            _buisnessAreaRepository.SaveChanges();

            return Ok();
        }
    }
}
