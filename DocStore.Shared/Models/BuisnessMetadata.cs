using System;
using System.Collections.Generic;
using System.Text;

namespace DocStore.Shared.Models
{
    public class BuisnessMetadata : EntityBase, IEquatable<BuisnessMetadata>
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

        public bool Equals(BuisnessMetadata other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null))
                return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other))
                return true;

            //Should be enought to compare equity
            return Key.Equals(other.Key) && Value.Equals(other.Value) && DocumentId.Equals(other.DocumentId);
        }
    }
}
