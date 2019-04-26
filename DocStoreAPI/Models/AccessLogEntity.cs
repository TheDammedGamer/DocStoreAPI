using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
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

    // Document = 10xx, DocumentVersion = 11xx, DocumentMetadata = 12xx,
    public enum AccessLogAction
    {
        DocumentCreate = 1001,
        DocumentRead = 1002,
        DocumentUpdate = 1003,
        DocumentArchive = 1004,
        DocumentDelete = 1005,
        DocumentMoved = 1006,
        DocumentLocked = 1007,
        DocumentUnLocked = 1008,

        DocumentVersionRead = 1101,
        DocumentVersionUpdate = 1102,
        DocumentVersionMove = 1103,
        DocumentVersionDelete = 1104,

        DocumentMetadataRead = 1201,
        DocumentMetadataUpdate = 1202,
        DocumentMetadataSearched = 1203,
        DocumentMetadataCreate = 1204,
    }
}
