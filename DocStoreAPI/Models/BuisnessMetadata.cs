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

    public class BuisnessMetadata : EntityBase
    {
        public int DocumentId { get; internal set; }
        public string Key { get; internal set; }
        public string Value { get; set; }

        public BuisnessMetadata()
        {

        }

        public BuisnessMetadata(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
