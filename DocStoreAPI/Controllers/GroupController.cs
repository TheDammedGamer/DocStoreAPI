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
    //TODO: Add Checks for Admin
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly GroupRepository _groupRepository;
        private readonly SecurityRepository _securityRepository;
        private const string _object = "Group";

        public GroupController(GroupRepository groupRepository, SecurityRepository securityRepository)
        {
            _groupRepository = groupRepository;
            _securityRepository = securityRepository;
        }

        // returns IEnumerable<GroupEntity>
        // GET: api/Group?page=1&perPage=20
        [HttpGet]
        public IActionResult Get([FromQuery]int page = 0, [FromQuery] int perPage = 25)
        {
            var currentUser = HttpContext.User.Identity.Name;
            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.Gate(GateType.Unathorised, AccessLogAction.GroupList, currentUser, _object, string.Empty);

            var groups = _groupRepository.ListP(out int pageCount, perPage, page);

            if (groups.Count() == 0)
                return _securityRepository.Gate(GateType.NotFound, AccessLogAction.GroupList, currentUser, _object, string.Empty);

            HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("TotalPages", pageCount.ToString()));
            _securityRepository.LogUserAction(currentUser, AccessLogAction.GroupList, string.Empty, _object, true);

            return Ok(groups);
        }

        // GET: api/Group/hrusers
        [HttpGet("{name}", Name = "Get")]
        public IActionResult Get(string name)
        {
            string currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.Gate(GateType.Unathorised, AccessLogAction.GroupReturn, currentUser, _object, name);

            var entity = _groupRepository.GetByName(name);
            
            if (string.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.Gate(GateType.NotFound, AccessLogAction.GroupReturn, currentUser, _object, name);

            _securityRepository.LogUserAction(currentUser, AccessLogAction.GroupReturn, name, _object, true);

            return Ok(entity);
        }

        // POST: api/Group
        [HttpPost]
        public IActionResult Post([FromBody] GroupEntity value)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.Gate(GateType.Unathorised, AccessLogAction.GroupCreate, currentUser, _object, string.Empty);

            _groupRepository.Add(value);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.GroupCreate, value.Name, _object, true);
            _groupRepository.SaveChanges();

            return Ok(value);
        }

        // PUT: api/Group/hrusers
        [HttpPut("{name}")]
        public IActionResult Put(string name, [FromBody] GroupEntity value)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.Gate(GateType.Unathorised, AccessLogAction.GroupUpdate, currentUser, _object, name);

            var entity = _groupRepository.GetByName(name);

            if (string.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.Gate(GateType.NotFound, AccessLogAction.GroupUpdate, currentUser, _object, name);

            _groupRepository.Edit(value);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.GroupUpdate, name, _object, true);
            _groupRepository.SaveChanges();

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/hrusers
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (!_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.Gate(GateType.Unathorised, AccessLogAction.GroupDelete, currentUser, _object, name);

            var entity = _groupRepository.GetByName(name);

            if (String.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.Gate(GateType.NotFound, AccessLogAction.GroupDelete, currentUser, _object, name);

            _groupRepository.DeleteById(entity.Id);
            _securityRepository.LogUserAction(currentUser, AccessLogAction.GroupDelete, name, _object, true);
            _groupRepository.SaveChanges();

            return Ok();
        }
    }
}
