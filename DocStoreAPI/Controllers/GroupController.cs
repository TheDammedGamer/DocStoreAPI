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
        // GET: api/Group
        [HttpGet]
        public IActionResult Get()
        {
            var currentUser = HttpContext.User.Identity.Name;
            if (_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.GroupList, _object, "NA");

            var groups = _groupRepository.List();

            return Ok(groups);
        }

        // GET: api/Group/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            try
            {
                var entity = _groupRepository.GetById(id);

                return Ok(entity);
            }
            catch (Exception ex)
            {
                string currentUser = HttpContext.User.Identity.Name;
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.GroupReturn, _object, id.ToString());
            }
        }

        // POST: api/Group
        [HttpPost]
        public IActionResult Post([FromBody] GroupEntity value)
        {
            var currentUser = HttpContext.User.Identity.Name;
            if (_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.GroupCreate, _object, "NA");

            _groupRepository.Add(value);
            _groupRepository.SaveChanges();

            return Ok(value); // returns Value with ID
        }

        // PUT: api/Group/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GroupEntity value)
        {
            var currentUser = HttpContext.User.Identity.Name;
            if (_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.GroupUpdate, _object, "NA");

            _groupRepository.Edit(value);
            _groupRepository.SaveChanges();

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var currentUser = HttpContext.User.Identity.Name;
            if (_securityRepository.UserIsAdmin(HttpContext))
                return _securityRepository.GateUnathorised(currentUser, AccessLogAction.GroupDelete, _object, "NA");

            _groupRepository.DeleteById(id);
            _groupRepository.SaveChanges();

            return Ok();
        }
    }
}
