using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.Server.Models.Stor
{
    public interface IStor
    {
        string ShortName { get; }
        Task<Stream> GetFileAsync(string fileName);
        Stream GetFile(string fileName);
        Task SetFileAsync(string fileName, Stream stream);
        void SetFile(string fileName, Stream stream);
        Task RemoveFileAsync(string fileName);
        void RemoveFile(string fileName);
        Task<string> GetFileHashAsync(string fileName);
        string GetFileHash(string fileName);
        void RenameFile(string oldFileName, string newfileName);
        Task RenameFileAsync(string oldFileName, string newfileName);
        bool TestConnection();
    }
}
