using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocStoreAPI.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DocStoreAPI.Services
{
    public class CustomAuthorizationService : IHostedService, IDisposable
    {
        private readonly DocStoreContext _context;
        private readonly ILogger _logger;

        private string _buisnessArea;
        private List<AccessControlEntity> _relevantACL;
        private List<GroupEntity> _relevantGroups = new List<GroupEntity>();

        public CustomAuthorizationService(ILogger<CustomAuthorizationService> logger, DocStoreContext docStoreContext)
        {
            _logger = logger;
            _context = docStoreContext;
        }

        public bool CheckAuthorization(string buisnessArea, ClaimsPrincipal user, string accessType)
        {
            throw new NotImplementedException();
            //_buisnessArea = buisnessArea;
            //switch (accessType.ToLower()) {
            //    case "create":
            //        _relevantACL = GetCreateACEByBuisnessArea();
            //        break;
            //    case "return":
            //        _relevantACL = GetReturnACEByBuisnessArea();
            //        break;
            //    case "update":
            //        _relevantACL = GetUpdateACEByBuisnessArea();
            //        break;
            //    case "delete":
            //        _relevantACL = GetDeleteACEByBuisnessArea();
            //        break;
            //    case "archive":
            //        _relevantACL = GetArchiveACEByBuisnessArea();
            //        break;
            //    default:
            //        throw new Exception("Unable to Check Authorization Type");
            //}

            //foreach (var ace in _relevantACL)
            //{
            //    var group = _context.GroupItems.First(grp => grp.Name == ace.GroupName);
            //    if (!_relevantGroups.Contains(group))
            //    {
            //        //Don't already have the group
            //        _relevantGroups.Add(group);
            //    }
            //}

            //if (_relevantGroups.Count == 0)
            //    return false;

            //if (user.Identity.AuthenticationType == "AzureAd")
            //{
            //    foreach (var grp in _relevantGroups)
            //    {
            //        if (user.IsInRole(grp.AzureName))
            //            return true;
            //    }
            //}
            //else
            //{
            //    foreach (var grp in _relevantGroups)
            //    {
            //        if (user.IsInRole(grp.ADName))
            //            return true;
            //    }
            //}

            //return false;
        }

        private List<AccessControlEntity> GetCreateACEByBuisnessArea()
        {
            return _context.AcessControlEntity.Where(ace => ace.BusinessArea == _buisnessArea && ace.Create).ToList();
        }
        private List<AccessControlEntity> GetReturnACEByBuisnessArea()
        {
            return _context.AcessControlEntity.Where(ace => ace.BusinessArea == _buisnessArea && ace.Return).ToList();
        }
        private List<AccessControlEntity> GetUpdateACEByBuisnessArea()
        {
            return _context.AcessControlEntity.Where(ace => ace.BusinessArea == _buisnessArea && ace.Update).ToList();
        }
        private List<AccessControlEntity> GetDeleteACEByBuisnessArea()
        {
            return _context.AcessControlEntity.Where(ace => ace.BusinessArea == _buisnessArea && ace.Delete).ToList();
        }
        private List<AccessControlEntity> GetArchiveACEByBuisnessArea()
        {
            return _context.AcessControlEntity.Where(ace => ace.BusinessArea == _buisnessArea && ace.Archvie).ToList();
        }

        public void Dispose()
        {
            _relevantGroups.Clear();
            _relevantACL.Clear();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
    }
}
