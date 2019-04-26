using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class DocumentModel
    {
        public MetadataEntity Metadata;
        public Stream DocumentData;
        //Only Return if Specifically asked for and User is Auditor
        public DocumentAudit AuditData;
    }
}
