using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DocStore.Server
{
    public static class Global
    {
        public static CredentialCache CredCache = new CredentialCache();
        public static string DefaultConnectionString;
    }
}
