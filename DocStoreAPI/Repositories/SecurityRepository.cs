using DocStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var aces = _context.AcessControlEntity.Where(ace => ace.BusinessArea == buisnessArea);

            switch (action.ToLower()) //Explicit cast as lowercase
            {
                case "c":
                    aces = aces.Where(ace => ace.Create);
                    break;
                case "r":
                    aces = aces.Where(ace => ace.Return);
                    break;
                case "u":
                    aces = aces.Where(ace => ace.Update);
                    break;
                case "d":
                    aces = aces.Where(ace => ace.Delete);
                    break;
                case "a":
                    aces = aces.Where(ace => ace.Archvie);
                    break;
                default:
                    throw new ArgumentException("unknown action passed through", nameof(action));
            }
            var acesL = aces.ToList();

            if (acesL.Count == 0)
                return false;

            foreach (var ace in acesL)
            {
                GroupEntity group;

                group = _context.GroupEntities.FirstOrDefault(ge => ge.Name == ace.GroupName);
                if (string.IsNullOrWhiteSpace(group.Name))
                    continue;

                //Valid Types
                //https://github.com/Microsoft/referencesource/blob/master/System.IdentityModel/System/IdentityModel/Claims/AuthenticationTypes.cs
                if (context.User.Identity.AuthenticationType == "Windows")
                {
                    if (context.User.IsInRole(group.ADName))
                        return true;
                }
                else
                {
                    if (context.User.IsInRole(group.AzureName))
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
