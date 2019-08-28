using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    // Document = 10xx, DocumentVersion = 11xx, DocumentMetadata = 12xx, Group = 20xx, AccessControlEntity = 30xx
    public enum AccessLogAction
    {
        DocumentCreate = 1001,
        DocumentRead = 1002,
        DocumentUpdate = 1003,
        DocumentArchive = 1004,
        DocumentDelete = 1005,
        DocumentMoved = 1006,
        DocumentLocked = 1007,
        DocumentUnlocked = 1008,

        DocumentVersionRead = 1101,
        DocumentVersionUpdate = 1102,
        DocumentVersionMove = 1103,
        DocumentVersionDelete = 1104,

        DocumentMetadataRead = 1201,
        DocumentMetadataUpdate = 1202,
        DocumentMetadataSearch = 1203,
        DocumentMetadataCreate = 1204,

        GroupCreate = 2001,
        GroupReturn = 2002,
        GroupUpdate = 2003,
        GroupArchive = 2004,
        GroupDelete = 2005,
        GroupList = 2006,

        ACECreate = 3001,
        ACEReturn = 3002,
        ACEUpdate = 3003,
        ACEArchive = 3004,
        ACEDelete = 3005,
        ACEList = 30056,

        BACreate = 4001,
        BAReturn = 4002,
        BAUpdate = 4003,
        BAArchive = 4004,
        BADelete = 4005,
        BAList = 40056,
    }
}
