using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocStore.Server.Models;
using DocStore.Server.Repositories;
using DocStore.Shared.Models;
using Microsoft.Extensions.Primitives;

namespace DocStore.API.Controllers
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
        // GET: api/BuisnessArea?page=1&perPage=20
        [HttpGet]
        public IActionResult Get([FromQuery]int page = 0, [FromQuery] int perPage = 25)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BAList, _objectType, string.Empty);

            int pageCount;
            var buisnessAreas = _buisnessAreaRepository.ListP(out pageCount, perPage, page);

            if (buisnessAreas.Count() == 0)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.BAList, _objectType, string.Empty);

            HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("TotalPages", pageCount.ToString()));

            _securityRepository.LogUserAction(currentUser, AccessLogAction.BAList, string.Empty, _objectType, true);
            _securityRepository.SaveChanges();

            return Ok(buisnessAreas);
        }

        // GET: api/BuisnessArea/HR
        [HttpGet("{name}", Name = "Get")]
        public IActionResult Get(string name)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BAReturn, _objectType, name);

            var entity = _buisnessAreaRepository.GetByName(name);

            if (String.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.BAReturn, _objectType, name);

            _securityRepository.LogUserAction(currentUser, AccessLogAction.BAReturn, name, _objectType, true);
            _securityRepository.SaveChanges();

            return Ok(entity);
        }

        // POST: api/BuisnessArea
        [HttpPost]
        public IActionResult Post([FromBody] BuisnessAreaEntity value)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BACreate, _objectType, string.Empty);

            _buisnessAreaRepository.Add(value);

            _securityRepository.LogUserAction(currentUser, AccessLogAction.BACreate, value.Name, _objectType, true);
            _buisnessAreaRepository.SaveChanges();

            return Ok(value);
        }

        // PUT: api/BuisnessArea/HR
        [HttpPut("{name}")]
        public IActionResult Put(string name, [FromBody] BuisnessAreaEntity value)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BAUpdate, _objectType, name);

            var entity = _buisnessAreaRepository.GetByName(name);

            if (String.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.BAUpdate, _objectType, name);

            _buisnessAreaRepository.Edit(value);

            _securityRepository.LogUserAction(currentUser, AccessLogAction.BAUpdate, name, _objectType, true);

            _buisnessAreaRepository.SaveChanges();

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/HR
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.BADelete, _objectType, name);

            var entity = _buisnessAreaRepository.GetByName(name);

            if (String.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.BADelete, _objectType, name);

            _buisnessAreaRepository.DeleteById(entity.Id);

            _securityRepository.LogUserAction(currentUser, AccessLogAction.BADelete, name, _objectType, true);

            _buisnessAreaRepository.SaveChanges();

            return Ok();
        }
    }
}
