using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DocStore.Shared.Models
{
    public class DocumentVersionEntity : EntityBase
    {
        [Required]
        public int DocumentId { get; set; }
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
    }
}
