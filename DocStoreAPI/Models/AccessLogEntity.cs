using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Models
{
    public class AccessLogEntity : EntityBase
    {
        public string User { get; protected set; }
        public AccessLogAction ActionId { get; protected set; }
        public string TargetId { get; protected set; }
        public string TargetType { get; protected set; }
        public bool WasSucessfull { get; protected set; }
        public DateTime LoggedAt { get; protected set; }

        public AccessLogEntity()
        {

        }

        public AccessLogEntity(string user, AccessLogAction actionId, string targetId, string targetType, bool wasSucessfull)
        {
            this.User = user ?? throw new ArgumentNullException(nameof(user));
            this.ActionId = actionId;
            this.TargetId = targetId;
            this.TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            this.WasSucessfull = wasSucessfull;
            this.LoggedAt = DateTime.UtcNow;
        }
    }

    
}
