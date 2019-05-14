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
            var entities = _accessRepository.List();

            return Ok(entities);
        }

        // GET: api/Access/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var result = _accessRepository.GetById(id);

            return Ok(result);
        }

        // POST: api/Access
        [HttpPost]
        public IActionResult Post([FromBody] AccessControlEntity value)
        {
            _accessRepository.Add(value);
            _accessRepository.SaveChanges();

            return Ok(value);
        }

        // PUT: api/Access/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] AccessControlEntity value)
        {
            _accessRepository.Edit(value);
            _accessRepository.SaveChanges();

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _accessRepository.DeleteById(id);
            _accessRepository.SaveChanges();

            return Ok();
        }
    }
}
