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
        public GroupController(GroupRepository groupRepository, SecurityRepository securityRepository)
        {
            _groupRepository = groupRepository;
            _securityRepository = securityRepository;
        }

        // GET: api/Group
        [HttpGet]
        public IEnumerable<GroupEntity> Get()
        {
            var groups = _groupRepository.List();

            return groups;
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
                return _securityRepository.GateNotFound(currentUser, AccessLogAction.GroupReturn, "GroupEntity", id.ToString());
            }
        }

        // POST: api/Group
        [HttpPost]
        public IActionResult Post([FromBody] GroupEntity value)
        {
            _groupRepository.Add(value);
            _groupRepository.SaveChanges();

            return Ok(value); // returns Value with ID
        }

        // PUT: api/Group/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GroupEntity value)
        {
            _groupRepository.Edit(value);
            _groupRepository.SaveChanges();

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _groupRepository.DeleteById(id);
            _groupRepository.SaveChanges();

            return Ok();
        }
    }
}
