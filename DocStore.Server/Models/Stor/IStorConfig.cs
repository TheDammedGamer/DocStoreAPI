using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DocStore.Server;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocStore.Server.Models.Stor
{
    public interface IStorConfig
    {
        string ShortName { get; set; }
        string StorType { get; }
        IStor GetStorFromConfig();
    }
}