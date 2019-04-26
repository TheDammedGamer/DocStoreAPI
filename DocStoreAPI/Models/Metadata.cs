using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class Metadata
    {
        public int Id { get; protected set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public string MD5Hash { get; set; } //D10CFB0D2819A862937A6D66E9CAE223
        public string StorName { get; set; }
        public string Extension { get; set; }
        public string BuisnessArea { get; set; } //Determines the ACE(s) relvant for the item

        public List<CustomMetadata> CustomMetadata { get; set; }

        //Only Access if URI Argument is added
        public List<OldVersion> OldVersions { get; set; }

        public bool IsLocked;
        public string LockedBy;
        public DateTime? LockedAt;
        public DateTime? LockExpiration;

        public bool isArchived;
        public string ArchivedBy;
        public DateTime? ArchivedAt;

        public string CreatedBy;
        public DateTime CreatedAt;
        public string LastUpdateBy;
        public DateTime LastUpdateAt;

        //Ctor
        public Metadata ()
        {

        }

        //Methods
        public OldVersion GetOldVersion()
        {
            return new OldVersion
            {
                DocumentId = this.Id,
                Name = this.Name,
                Version = this.Version,
                MD5Key = this.MD5Hash,
                StorName = this.StorName,
                Extension = this.Extension
            };

        }
        public string GetFileName()
        {
            return String.Format("{0}.{1}", Name, Extension);
        }

        public string GetServerName()
        {
            return String.Format("{0}.v{1}.{2}.{3}", Id, Version, Name, Extension);
        }
    }

    public class CustomMetadata
    {
        public string Key;
        public string Value;

        public CustomMetadata()
        {

        }
    }

    public class DocumentAudit
    {
        public List<ChangeLogItem> ChangeLog; //Change log for reporting and record keeping (e.g. GDPR)
        public List<AccessLogItem> AccessLog; //Access log for the same as above
    }

    public class OldVersion
    {
        public int Id;
        public int DocumentId;
        public string Name;
        public int Version;
        public string MD5Key; //D10CFB0D2819A862937A6D66E9CAE223
        public string StorName;
        public string Extension;

        public OldVersion ()
        {

        }
    }

    public class ChangeLogItem
    {
        public int Id;
        public int MetadataId;

        public string Key;
        public string Value;
        public int OldVersionID;

        public string UpdatedBy;
        public DateTime UpdatedAt;
    }

    public class AccessLogItem
    {
        public int Id;
        public string User;
        public int ActionId;
        public string Target;
        public bool isSucessfull;
        public int ChangeId;
        public DateTime LoggedAt;
    }

    //public enum AccessLogAction //1xxx = Document, 2xxx = Security Groups, 3xxx = ACE
    //{
    //    DocumentLocked = 1001,
    //    DocumentDownloaded = 1002,
    //    DocumentArchived = 1003,
    //    DocumentDeleted = 1004,
    //    DocumentDeletedArchive = 1005,
    //    DcoumentMoved = 1006,
    //    DocumentCreated = 1007,
    //    DocumentNewVersionUploaded = 1008,
    //    DocumentMetadataViewed = 1009,
    //    DocumentMetadataChanged = 1010,

    //    GroupCreated = 2001,
    //    GroupIsAdmin = 2002,
    //    GroupIsNotAdmin = 2003,
    //    GroupIsAudit = 2004,
    //    GroupIsNotAudit = 2005,
    //    GroupMembershipAdded = 2006,
    //    GroupMembershipRemoved = 2007,
    //    GroupIsSubbedToAD = 2008,
    //    GroupIsNotSubbedToAD = 2009,
    //    GroupIsSubbedToAZ = 2010,
    //    GroupIsUnSubbedToAz = 2011,

    //    ACECreated = 3001,
    //    ACEAcessChanged = 3002,
    //    ACERemoved = 3003,
    //}
}
