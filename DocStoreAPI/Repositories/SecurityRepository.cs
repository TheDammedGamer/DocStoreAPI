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

namespace DocStoreAPI.Repositories
{
    public class SecurityRepository
    {
        private readonly DocStoreContext _context;
        private readonly ILogger _logger;

        public SecurityRepository(ILogger<SecurityRepository> logger, DocStoreContext docStoreContext)
        {
            _logger = logger;
            _context = docStoreContext;
        }

        //action could be any of c,r,u,d,a
        public bool UserIsAuthorisedByBuisnessArea(HttpContext context, string buisnessArea, string action)
        {
            var aces = _context.BuisnessAreas.First(bae => bae.Name == buisnessArea).RelevantAccessControlEntities;
            switch (action.ToLower())
            {
                case "c":
                    aces = aces.Where(ace => ace.Create).ToList();
                    break;
                case "r":
                    aces = aces.Where(ace => ace.Return).ToList();
                    break;
                case "u":
                    aces = aces.Where(ace => ace.Update).ToList();
                    break;
                case "d":
                    aces = aces.Where(ace => ace.Delete).ToList();
                    break;
                case "a":
                    aces = aces.Where(ace => ace.Archive).ToList();
                    break;
                default:
                    throw new ArgumentException("unknown action passed through", nameof(action));
            }

            if (aces.Count == 0)
                return false;

            foreach (var ace in aces)
            {
                var grp = ace.Group;

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
            return false;
        }

        public bool UserIsAuthorisedByBuisnessAreas(HttpContext context, string buisnessArea1, string buisnessArea2, string action)
        {
            var baes = _context.BuisnessAreas.Where(bae => bae.Name == buisnessArea1 || bae.Name == buisnessArea2).Include(bae => bae.RelevantAccessControlEntities);

            List<AccessControlEntity> aces = new List<AccessControlEntity>();

            foreach (var ba in baes)
            {
                aces.AddRange(ba.RelevantAccessControlEntities);
            }

            switch (action.ToLower()) //Explicit cast as lowercase
            {
                case "c":
                    aces = aces.Where(ace => ace.Create).ToList();
                    break;
                case "r":
                    aces = aces.Where(ace => ace.Return).ToList();
                    break;
                case "u":
                    aces = aces.Where(ace => ace.Update).ToList();
                    break;
                case "d":
                    aces = aces.Where(ace => ace.Delete).ToList();
                    break;
                case "a":
                    aces = aces.Where(ace => ace.Archive).ToList();
                    break;
                default:
                    throw new ArgumentException("unknown action passed through", nameof(action));
            }

            if (aces.Count == 0)
                return false;

            foreach (var ace in aces)
            {
                var grp = ace.Group;

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
            return false;
        }


        public void LogUserAction(string user, AccessLogAction actionId, int targetId, string targetType, bool success)
        {
            _context.AccessLogEntities.Add(new AccessLogEntity(user, actionId, targetId.ToString(), targetType, success));
            _context.SaveChanges();
        }

        public void LogUserAction(string user, AccessLogAction actionId, string targetId, string targetType, bool success)
        {
            _context.AccessLogEntities.Add(new AccessLogEntity(user, actionId, targetId, targetType, success));
            _context.SaveChanges();
        }
    }
}
