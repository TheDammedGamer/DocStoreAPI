using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class CustomMetadataEntity : EntityBase
    {
        public int DocumentId { get; protected set; }
        public string Key { get; protected set; }
        public string Value { get; set; }

        public CustomMetadataEntity()
        {

        }

        public CustomMetadataEntity(int documentId, string key, string value)
        {
            this.DocumentId = documentId;
            this.Key = key;
            this.Value = value;
        }
    }
}
