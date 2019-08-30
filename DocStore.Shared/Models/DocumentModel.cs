using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocStore.Shared.Models
{
    public class DocumentModel
    {
        public MetadataEntity Metadata;
        public Stream DocumentData;
    }
}
