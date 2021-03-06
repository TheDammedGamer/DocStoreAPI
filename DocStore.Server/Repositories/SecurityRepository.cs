﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using DocStore.Server.Models;
using DocStore.Shared.Models;
using DocStore.Shared.Models.Search;

namespace DocStore.Server.Repositories
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

        private GroupEntity GetGroupByName(string name)
        {
            return _context.GroupEntities.First(g => g.Name == name);
        }

        private List<AccessControlEntity> GetACEByBA(string name)
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
        public void LogUserSearch(string user, MetadataSearch search)
        {
            _context.SearchLogs.Add(new SearchLogEntity(user, search));
        }


        public bool UserIsAdmin(HttpContext context)
        {
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

        public bool UserIsAuthorisedByBuisnessAreas(HttpContext context, AuthActions action, params string[] buisnessArea)
        {
            if (buisnessArea.Count() == 0)
                return false;

            List<AccessControlEntity> aces = new List<AccessControlEntity>();

            var query = _context.AcessControlEntity.AsQueryable();

            foreach (var ba in buisnessArea)
            {
                var tmpquery = query.Where(bac => bac.BusinessArea == ba);

                switch (action)
                {
                    case AuthActions.Create:
                        tmpquery = tmpquery.Where(ace => ace.Create);
                        break;
                    case AuthActions.Return:
                        tmpquery = tmpquery.Where(ace => ace.Return);
                        break;
                    case AuthActions.Update:
                        tmpquery = tmpquery.Where(ace => ace.Update);
                        break;
                    case AuthActions.Delete:
                        tmpquery = tmpquery.Where(ace => ace.Delete);
                        break;
                    case AuthActions.Archive:
                        tmpquery = tmpquery.Where(ace => ace.Archive);
                        break;
                    case AuthActions.Supervisor:
                        tmpquery = tmpquery.Where(ace => ace.Supervisor);
                        break;
                }

                aces.AddRange(tmpquery.ToList());
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
            if (UserIsAdmin(context))
                return true;
            else
                return false;
        }

        public IActionResult GateUnathorised(string username, AccessLogAction ala, string objectType, string objectValue)
        {
            LogUserAction(username, ala, objectValue, objectType, false);
            _logger.LogDebug((int)ala, "Unathorised to access '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
            SaveChanges();
            return new UnauthorizedResult();
        }

        public IActionResult GateNotFound(string username, AccessLogAction ala, string objectType, string objectValue)
        {
            LogUserAction(username, ala, objectValue, objectType, false);
            _logger.LogDebug((int)ala, "Failed to find '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
            SaveChanges();
            return new NotFoundObjectResult(objectValue);
        }

        public IActionResult GateCannotLock(string username, string objectType, string objectValue)
        {
            LogUserAction(username, AccessLogAction.DocumentLocked, objectValue, objectType, false);
            _logger.LogDebug((int)AccessLogAction.DocumentLocked, "Failed to lock '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
            SaveChanges();
            return new UnauthorizedResult();
        }
        public IActionResult GateCannotUnlock(string username, string objectType, string objectValue)
        {
            LogUserAction(username, AccessLogAction.DocumentUnlocked, objectValue, objectType, false);
            _logger.LogDebug((int)AccessLogAction.DocumentUnlocked, "Failed to unlock '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
            SaveChanges();
            return new UnauthorizedResult();
        }

        public IActionResult GateDocumentLockedByAnotherUser(string username, string objectType, string objectValue)
        {
            LogUserAction(username, AccessLogAction.DocumentUpdate, objectValue, objectType, false);
            _logger.LogDebug((int)AccessLogAction.DocumentUpdate, "Failed to update '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
            SaveChanges();

            var res = new ContentResult
            {
                StatusCode = 409,
                ContentType = "text/plain",
                Content = "Document is Locked by another User"
            };

            return res;
        }

        public IActionResult Gate(GateType gate, AccessLogAction logAction, string username, string objectType, string objectValue)
        {
            switch (gate)
            {
                case GateType.LockedByAnotherUser:
                    LogUserAction(username, logAction, objectValue, objectType, false);
                    SaveChanges();
                    _logger.LogDebug((int)logAction, "Failed to update '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
                    var res = new ContentResult
                    {
                        StatusCode = 409,
                        ContentType = "text/plain",
                        Content = "Document is Locked by another User"
                    };

                    return res;
                case GateType.CannotLock:
                    LogUserAction(username, logAction, objectValue, objectType, false);
                    SaveChanges();
                    _logger.LogDebug((int)logAction, "Failed to lock '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
                    return new UnauthorizedResult();
                case GateType.CannotUnlock:
                    LogUserAction(username, logAction, objectValue, objectType, false);
                    SaveChanges();
                    _logger.LogDebug((int)logAction, "Failed to unlock '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
                    return new UnauthorizedResult();
                case GateType.NotFound:
                    LogUserAction(username, logAction, objectValue, objectType, false);
                    SaveChanges();
                    _logger.LogDebug((int)logAction, "Failed to find '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
                    return new NotFoundObjectResult(objectValue);
                case GateType.Unathorised:
                    LogUserAction(username, logAction, objectValue, objectType, false);
                    SaveChanges();
                    _logger.LogDebug((int)logAction, "Unathorised to access '{0}' with identifier '{1}' for user '{2}'", objectType, objectValue, username);
                    return new UnauthorizedResult();
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public enum GateType
    {
        LockedByAnotherUser,
        CannotLock,
        CannotUnlock,
        NotFound,
        Unathorised
    }
}
