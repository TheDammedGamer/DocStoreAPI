using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models
{
    public class DocumentVersionEntity : EntityBase
    {
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public string MD5Hash { get; set; } //D10CFB0D2819A862937A6D66E9CAE223
        public string StorName { get; set; }
        public string Extension { get; set; }

        public DocumentVersionEntity()
        {

        }

        public static DocumentVersionEntity FromMetadata(MetadataEntity meta)
        {
            var result = new DocumentVersionEntity
            {
                DocumentId = meta.Id,
                Name = meta.Name,
                Version = meta.Version,
                MD5Hash = meta.MD5Hash,
                StorName = meta.StorName,
                Extension = meta.Extension
            };

            return result;
        }

        public string GetFileName()
        {
            return string.Format("{0}.{1}", this.Name, this.Extension);
        }
        public string GetServerFileName()
        {
            return string.Format("{0}.v{1}.{2}.{3}",
                this.DocumentId.ToString(), this.Version.ToString(), this.Name, this.Extension);
        }
    }
}
