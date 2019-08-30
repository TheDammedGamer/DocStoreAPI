using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocStore.API.Models;
using DocStore.API.Repositories;
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
            if (_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.GroupList, _object, string.Empty);


            var groups = _groupRepository.ListP(out int pageCount, perPage, page);

            if (groups.Count() == 0)
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.GroupList, _object, string.Empty);

            _securityRepository.LogUserAction(currentUser, AccessLogAction.GroupList, string.Empty, _object, true);

            HttpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("TotalPages", pageCount.ToString()));

            return Ok(groups);
        }

        // GET: api/Group/hrusers
        [HttpGet("{name}", Name = "Get")]
        public IActionResult Get(string name)
        {
            string currentUser = HttpContext.User.Identity.Name;

            if (_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.GroupReturn, _object, name);

            var entity = _groupRepository.GetByName(name);
            
            if (string.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.GroupReturn, _object, name);

            _securityRepository.LogUserAction(currentUser, AccessLogAction.GroupReturn, name, _object, true);

            return Ok(entity);
        }

        // POST: api/Group
        [HttpPost]
        public IActionResult Post([FromBody] GroupEntity value)
        {
            var currentUser = HttpContext.User.Identity.Name;

            if (_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.GroupCreate, _object, string.Empty);

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

            if (_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.GroupUpdate, _object, name);

            var entity = _groupRepository.GetByName(name);

            if (string.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.GroupUpdate, _object, name);

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

            if (_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.GroupDelete, _object, name);

            var entity = _groupRepository.GetByName(name);

            if (String.IsNullOrWhiteSpace(entity.Name))
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.GroupDelete, _object, name);

            _groupRepository.DeleteById(entity.Id);

            _securityRepository.LogUserAction(currentUser, AccessLogAction.GroupDelete, name, _object, true);

            _groupRepository.SaveChanges();

            return Ok();
        }
    }
}
