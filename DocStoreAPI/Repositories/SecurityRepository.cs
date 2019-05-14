using DocStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using DocStoreAPI.Controllers;

namespace DocStoreAPI.Repositories
{
    public class SecurityRepository : BaseRepository
    {
        private readonly DocStoreContext _context;
        private readonly ILogger _logger;
        private readonly Admins _admins;

        public SecurityRepository(ILogger<SecurityRepository> logger, DocStoreContext docStoreContext, Admins adminList)
        {
            _logger = logger;
            _context = docStoreContext;
            _admins = adminList;
        }

        public GroupEntity GetGroupByName(string name)
        {
            return _context.GroupEntities.First(g => g.Name == name);
        }

        public List<AccessControlEntity> GetACEByBA(string name)
        {
            return _context.AcessControlEntity.Where(ace => ace.BusinessArea == name).ToList();
        }


        public void LogUserAction(string user, AccessLogAction actionId, int targetId, string targetType, bool success)
        {
            _context.AccessLogEntities.Add(new AccessLogEntity(user, actionId, targetId.ToString(), targetType, success));
        }

        public void LogUserAction(string user, AccessLogAction actionId, string targetId, string targetType, bool success)
        {
            _context.AccessLogEntities.Add(new AccessLogEntity(user, actionId, targetId, targetType, success));
        }
        public void LogUserAction(HttpContext context, AccessLogAction actionId, string targetId, string targetType, bool success)
        {
            _context.AccessLogEntities.Add(new AccessLogEntity(context.User.Identity.Name, actionId, targetId, targetType, success));
        }


        public bool UserIsAuthorisedByBuisnessAreas(HttpContext context, AuthActions action, params string[] buisnessArea)
        {
            if (buisnessArea.Count() == 0)
                return false;

            List<AccessControlEntity> aces = new List<AccessControlEntity>();

            foreach (var ba in buisnessArea)
            {
                aces.AddRange(GetACEByBA(ba));
            }

            switch (action)
            {
                case AuthActions.Create:
                    aces = aces.Where(ace => ace.Create).ToList();
                    break;
                case AuthActions.Return:
                    aces = aces.Where(ace => ace.Return).ToList();
                    break;
                case AuthActions.Update:
                    aces = aces.Where(ace => ace.Update).ToList();
                    break;
                case AuthActions.Delete:
                    aces = aces.Where(ace => ace.Delete).ToList();
                    break;
                case AuthActions.Archive:
                    aces = aces.Where(ace => ace.Archive).ToList();
                    break;
            }

            if (aces.Count == 0)
                return false;

            foreach (var ace in aces)
            {
                var grp = GetGroupByName(ace.Group);
                //Valid Types
                //https://github.com/Microsoft/referencesource/blob/master/System.IdentityModel/System/IdentityModel/Claims/AuthenticationTypes.cs
                if (context.User.Identity.AuthenticationType == "Windows")
                {
                    if (context.User.IsInRole(grp.ADName))
                        return true;
                }
                else
                {
                    if (context.User.IsInRole(grp.AzureName))
                        return true;
                }
            }

            //Check if User is Admin
            if (context.User.Identity.AuthenticationType == "Windows")
            {
                foreach (var item in _admins.ADAdminGroupNames)
                {
                    if (context.User.IsInRole(item))
                        return true;
                }
            }
            else
            {
                foreach (var item in _admins.AZAdminGroupNames)
                {
                    if (context.User.IsInRole(item))
                        return true;
                }
            }
            return false;
        }

        public IActionResult GateUnathorised(HttpContext context, AccessLogAction ala, string objectType, string objectValue)
        {
            LogUserAction(context, ala, objectValue, objectType, false);
            _logger.LogInformation((int)ala, "Unathorised to access object '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, context.User.Identity.Name);
            this.SaveChanges();
            return new UnauthorizedResult();
        }

        public IActionResult GateNotFound(HttpContext context, AccessLogAction ala, string objectType, string objectValue)
        {
            LogUserAction(context, ala, objectValue, objectType, false);
            _logger.LogInformation((int)ala, "Failed to find object '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, context.User.Identity.Name);
            this.SaveChanges();
            return new NotFoundObjectResult(objectValue);
        } 


        public IActionResult GateUnathorised(string username, AccessLogAction ala, string objectType, string objectValue)
        {
            LogUserAction(username, ala, objectValue, objectType, false);
            _logger.LogInformation((int)ala, "Unathorised to access object '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
            this.SaveChanges();
            return new UnauthorizedResult();
        }

        public IActionResult GateNotFound(string username, AccessLogAction ala, string objectType, string objectValue)
        {
            LogUserAction(username, ala, objectValue, objectType, false);
            _logger.LogInformation((int)ala, "Failed to find object '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
            this.SaveChanges();
            return new NotFoundObjectResult(objectValue);
        }

        public IActionResult GateCannotLock(string username, string objectType, string objectValue)
        {
            LogUserAction(username, AccessLogAction.DocumentLocked, objectValue, objectType, false);
            _logger.LogInformation((int)AccessLogAction.DocumentLocked, "Failed to Lock object '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
            this.SaveChanges();
            return new UnauthorizedResult();
        }
        public IActionResult GateCannotUnlock(string username, string objectType, string objectValue)
        {

            LogUserAction(username, AccessLogAction.DocumentUnlocked, objectValue, objectType, false);
            _logger.LogInformation((int)AccessLogAction.DocumentUnlocked, "Failed to Unlock object '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
            this.SaveChanges();
            return new UnauthorizedResult();
        }
    }
}
