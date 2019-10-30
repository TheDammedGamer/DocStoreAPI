using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;

namespace DocStore.Server.Models.Stor
{
    public class FileShareStorConfig : IStorConfig
    {
        public bool RequiresAuth { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string BasePath { get; set; }

        public string StorType { get => "FileShareStor"; }
        public string ShortName { get; set; }

        public FileShareStorConfig()
        {

        }

        public IStor GetStorFromConfig()
        {
            if (RequiresAuth)
            {
                NetworkCredential netCred = new NetworkCredential(UserName, Password, Domain);
                return new FileShareStor(BasePath, netCred, ShortName);
            }
            else
            {
                return new FileShareStor(BasePath, ShortName);
            }
        }
    }
}
