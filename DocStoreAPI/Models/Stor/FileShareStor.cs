using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DocStoreAPI.Models.Stor
{
    public class FileShareStor : IStor
    {
        // Properties
        private string _basePath;

        public string ShortName { get; private set; }

        // ctor
        public FileShareStor(string basePath, string shortName)
        {
            _basePath = basePath.TrimEnd('\\');
            ShortName = shortName;
        }
        public FileShareStor(string basePath, NetworkCredential cred, string shortName)
        {
            _basePath = basePath.TrimEnd('\\');

            string machineName = basePath.TrimStart('\\');
            machineName = @"\" + machineName.Substring(0, machineName.IndexOf('\\'));

            Global.CredCache.Add(new Uri(machineName), "Basic", cred);
            ShortName = shortName;
        }

        // Methods
        private string GetFilePath(string fileName)
        {
            return String.Format("{0}\\{1}", _basePath, fileName);
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            var path = GetFilePath(fileName);
            try
            {
                byte[] bytes = await File.ReadAllBytesAsync(path);
                MemoryStream str = new MemoryStream(bytes, false);
                bytes = null;
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to read from file: '{0}'", path), ex);
            }
        }
        public Stream GetFile(string fileName)
        {
            var path = GetFilePath(fileName);
            try
            {
                return File.OpenRead(path);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to read from file: '{0}'", path), ex);
            }
        }

        public async Task SetFileAsync(string fileName, Stream stream)
        {
            var path = GetFilePath(fileName);
            try
            {
                using (FileStream DestinationStream = File.Create(path))
                {
                    await stream.CopyToAsync(DestinationStream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to write to file: '{0}'", path), ex);
            }
        }
        public void SetFile(string fileName, Stream stream)
        {
            var path = GetFilePath(fileName);
            try
            {
                using (var writer = File.OpenWrite(path))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(writer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to write to file: '{0}'", path), ex);
            }
        }

        public async Task RemoveFileAsync(string fileName)
        {
            var path = GetFilePath(fileName);

            await Task.Run(() => { File.Delete(path); });
        }
        public void RemoveFile(string fileName)
        {
            var path = GetFilePath(fileName);
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error when removing file: '{0}'", path), ex);
            }
        }

        public async Task<string> GetFileHashAsync(string fileName)
        {
            var path = GetFilePath(fileName);
            return await Task.Run(() =>
            {
                using (var stream = File.OpenRead(path))
                {
                    using (var md5 = MD5.Create())
                    {
                        var hash = md5.ComputeHash(stream);
                        return Convert.ToBase64String(hash);
                    }
                }
            });
        }
        public string GetFileHash(string fileName)
        {
            var path = GetFilePath(fileName);
            using (var stream = File.OpenRead(path))
            {
                using (var md5 = MD5.Create())
                {
                    var hash = md5.ComputeHash(stream);
                    return Convert.ToBase64String(hash);
                }
            }
        }

        public bool TestConnection()
        {
            var path = GetFilePath("Test.txt");
            try
            {
                // Delete does not throw an exception if the file does not exist but will if unable to access the file.
                File.Delete(path);
                using (FileStream Test = File.Create(path))
                {
                    if (Test.CanRead && Test.CanWrite)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to Connect to File stor '{0}'", ShortName), ex);
            }
        }
    }
}