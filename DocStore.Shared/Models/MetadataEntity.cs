using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DocStore.Shared.Models
{
    public class MetadataEntity : EntityBase, IEquatable<MetadataEntity>
    {
        //Id is From Entity Base
        [Required]
        public string Name { get; set; }
        [Required]
        public int Version { get; set; }
        [Required]
        public string MD5Hash { get; set; } //D10CFB0D2819A862937A6D66E9CAE223
        [Required]
        public string StorName { get; set; }
        [Required]
        public string Extension { get; set; }
        [Required]
        public string BuisnessArea { get; set; }

        //public List<BuisnessMetadata> BuisnessMetadata { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public List<DocumentVersionEntity> Versions { get; set; }

        [Required]
        public LockState Locked { get; set; }
        [Required]
        public ArchiveState Archive { get; set; }
        [Required]
        public UpdateState Created { get; set; }
        [Required]
        public UpdateState LastUpdate { get; set; }
        [Required]
        public DateTime LastViewed { get; set; }

        public MetadataEntity()
        {

        }

        public MetadataEntity(string fileName, int version, string extension, string buisnessArea, string storName, string userName)
        {
            this.Name = fileName;
            this.Version = version;
            this.Extension = extension;
            this.BuisnessArea = buisnessArea;
            this.StorName = storName;
            this.Versions = new List<DocumentVersionEntity>();
            this.Metadata = new Dictionary<string, string>();
            this.Locked = new LockState();
            this.Archive = new ArchiveState();
            this.LastUpdate = new UpdateState(userName);
            this.Created = new UpdateState(userName);
            this.LastViewed = DateTime.UtcNow;
        }

        public string GetFileName()
        {
            return string.Format("{0}.{1}", this.Name, this.Extension);
        }
        public string GetServerFileName()
        {
            return string.Format("{0}.v{1}.{2}.{3}",
                this.Id.ToString(), this.Version.ToString(), this.Name, this.Extension);
        }

        public bool Equals(MetadataEntity other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null))
                return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other))
                return true;

            //Should be enough to compare equity
            return Version.Equals(other.Version) && Name.Equals(other.Name) && StorName.Equals(other.StorName) && Extension.Equals(other.Extension) && BuisnessArea.Equals(other.BuisnessArea) && Metadata.Equals(other.Metadata) && Created.Equals(other.Created);
        }

        public bool ContainsMetadata(string cKey, string cValue)
        {
            if (string.IsNullOrWhiteSpace(cKey))
                throw new Exception("Comparison Key is null, empty or whitespace");
            if (string.IsNullOrWhiteSpace(cValue))
                throw new Exception("Comparison Value is null, empty or whitespace");

            if (Metadata[cKey] == cValue)
                return true;
            else
                return false;
        }
        
        public bool ContainsMetadataKey(string cKey)
        {
            if (string.IsNullOrWhiteSpace(cKey))
                throw new Exception("Comparison Key is null, empty or whitespace");

            if (Metadata.ContainsKey(cKey))
                return true;
            else
                return false;
        }
        
        public bool ContainsMetadataValue(string cValue)
        {
            if (string.IsNullOrWhiteSpace(cValue))
                throw new Exception("Comparison Value is null, empty or whitespace");

            if (Metadata.ContainsValue(cValue))
                return true;
            else
                return false;
        }
    }
}
