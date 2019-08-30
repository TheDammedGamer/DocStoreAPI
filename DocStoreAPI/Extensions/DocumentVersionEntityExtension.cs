using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocStore.Shared.Models;

namespace DocStore.API.Extensions
{
    public static class DocumentVersionEntityExtension
    {
        public static string GetServerFileName(this DocumentVersionEntity versionEntity)
        {
            return string.Format("{0}.v{1}.{2}.{3}",
                versionEntity.DocumentId.ToString(), versionEntity.Version.ToString(), versionEntity.Name, versionEntity.Extension);
        }
    }
}
