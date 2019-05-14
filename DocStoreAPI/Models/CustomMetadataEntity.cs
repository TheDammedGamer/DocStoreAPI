using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class CustomMetadataEntity : EntityBase
    {
        public MetadataEntity Document { get; protected set; }
        public string Key { get; protected set; }
        public string Value { get; set; }

        public CustomMetadataEntity()
        {

        }

        public CustomMetadataEntity(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
