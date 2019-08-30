using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DocStore.API.Models;
using DocStore.API.Models.Stor;
using DocStore.API.Shared;
using DocStore.Shared.Models;
using Microsoft.AspNetCore.Authorization;


namespace DocStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AzAppUser", AuthenticationSchemes ="Jwt,Ntlm")]
    public class OldDocumentController : ControllerBase
    {
        private readonly ILogger _logger;

        public OldDocumentController(ILogger<OldDocumentController> logger)
        {
            _logger = logger;
        }


        // GET: api/Document
        [HttpGet]
        public IEnumerable<string> Get()
        {

            System.Security.Claims.ClaimsPrincipal usr = HttpContext.User;
            usr.IsInRole("fuckknows");

            return new string[] { "value1", "value2" };

        }

        // GET: api/Document/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {


            throw new NotImplementedException();
            //string apiAuthKey;
            ////TODO: Check Access
            //string[] AuthHeader = Request.Headers["Authorization"].ToString().Split(' ');
            //if (AuthHeader[0].Trim().ToLower() == "apiauth")
            //{
            //    //apiAuthKey Should look like 'Key:Username' with seperator
            //    apiAuthKey = GenericHelpers.Base64Decode(AuthHeader[1].Trim());

            //    if (!DatabaseActions.CheckLoginKey(apiAuthKey))
            //    {
            //        //Login Key Check has failed
            //        return Unauthorized();
            //    }

            //    _logger.Log(LogLevel.Information, "Getting Metadata of document '{0}' by user '{1}'.", id, (apiAuthKey.Split(':'))[1]);
            //    Metadata docDetails; //= DatabaseActions.GetDocumentMetadata(id);

            //    //Compare Access w/Metadata

            //    if (DatabaseActions.CheckAuthorisation(apiAuthKey, docDetails.BuisnessArea, "read"))
            //    {
            //        _logger.Log(LogLevel.Information, "Request for Metadata of document '{0}' by user '{1}' is authorisied.", id, (apiAuthKey.Split(':'))[1]);
            //        //Continue with Request as Message is authorised

            //        //TODO: Record Sucessfull Access

            //        //Return Metadata
            //        return Ok(docDetails);
            //    }
            //    else
            //    {
            //        _logger.Log(LogLevel.Information, "Request for metadata of document '{0}' by user '{1}' is not authorisied.", id, (apiAuthKey.Split(':'))[1]);
            //        //Request is Authenticated but not Authorised

            //        //TODO: Record failed access

            //        //Return Error
            //        return StatusCode(401, "User does not have Authorisation to access this resource.");
            //    }

            //}
            //else if (AuthHeader[0].Trim().ToLower() == "NTLM")
            //{
            //    //Do some stuff with NTLM to get authed
            //    throw new NotImplementedException();
            //}
            //else
            //{
            //    return Unauthorized();
            //}
        }

        // GET: api/Document/5/Download
        [HttpGet("{id}/Download", Name = "Get")]
        public IActionResult Download(int id)
        {
            throw new NotImplementedException();
        }

        // POST: api/Document
        [HttpPost]
        public void Post([FromBody] string value)
        {
            throw new NotImplementedException();
        }

        // PUT: api/Document/5/Lock
        [HttpPut("{id}/Lock")]
        public void Lock(int id, [FromBody] string value)
        {
            throw new NotImplementedException();
        }


        // PUT: api/Document/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/Document/5/Archive
        [HttpDelete("{id}/Archive")]
        public void Archive(int id)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/Document/5/Delete
        [HttpDelete("{id}/Delete")]
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
