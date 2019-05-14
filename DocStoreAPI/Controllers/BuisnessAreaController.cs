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
        public BuisnessAreaController(BuisnessAreaRepository repository, SecurityRepository securityRepository)
        {
            _buisnessAreaRepository = repository;
            _securityRepository = securityRepository;
        }
        // GET: api/BuisnessArea
        [HttpGet]
        public IActionResult Get()
        {
            var buisnessAreas = _buisnessAreaRepository.List();

            return Ok(buisnessAreas);
        }

        // GET: api/BuisnessArea/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var ba = _buisnessAreaRepository.GetById(id);

            return Ok(ba);
        }

        // POST: api/BuisnessArea
        [HttpPost]
        public IActionResult Post([FromBody] BuisnessAreaEntity value)
        {
            _buisnessAreaRepository.Add(value);
            _buisnessAreaRepository.SaveChanges();

            return Ok(value);
        }

        // PUT: api/BuisnessArea/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] BuisnessAreaEntity value)
        {
            _buisnessAreaRepository.Edit(value);
            _buisnessAreaRepository.SaveChanges();

            return Ok(value);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _buisnessAreaRepository.DeleteById(id);
            _buisnessAreaRepository.SaveChanges();

            return Ok();
        }
    }
}
