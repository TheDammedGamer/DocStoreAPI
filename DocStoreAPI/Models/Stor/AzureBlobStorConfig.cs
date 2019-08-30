using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Models.Stor
{
    public class AzureBlobStorConfig : IStorConfig
    {
        public string ContainerName { get; set; }
        public string AccessKey { get; set; }
        public string AccountName { get; set; }

        public string StorType { get => "AzureBlobStor"; }
        public string ShortName { get; set; }

        public AzureBlobStorConfig()
        {

        }

        public IStor GetStorFromConfig()
        {
            return new AzureBlobStor(ShortName, ContainerName, AccessKey, AccountName);
        }
    }
}
