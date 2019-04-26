using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocStoreAPI.Models.Stor
{
    public class AzureBlobStor : IStor
    {
        private string _containerName { get; set; }
        private string _accessKey { get; set; }
        private string _accountName { get; set; }

        public string ShortName { get; set; }

        public AzureBlobStor(string storName, string contianerName, string accessKey, string accountName)
        {
            this.ShortName = storName;
            this._containerName = contianerName;
            this._accessKey = accessKey;
            this._accountName = accountName;
        }

        private CloudBlobContainer GetBlobContainer()
        {
            var cloudStorageAccount = new CloudStorageAccount(new StorageCredentials(_accountName, _accessKey), true);
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(_containerName);
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            try
            {
                var containerRef = GetBlobContainer();
                var blobFileRef = await containerRef.GetBlobReferenceFromServerAsync(fileName);

                MemoryStream target = new MemoryStream();
                await blobFileRef.DownloadToStreamAsync(target);

                //go back to the Start after downloading the stream
                target.Position = 0;
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to connect to Azure Blob Storage container '{0}'", ShortName), ex);
            }
        }
        public Stream GetFile(string fileName)
        {
            try
            {
                var containerRef = GetBlobContainer();
                var blobFileRef = containerRef.GetBlobReference(fileName);

                //maybe this works maybe this doesn't work probs not going to use it
                MemoryStream target = new MemoryStream();
                var tsk = blobFileRef.DownloadToStreamAsync(target);
                tsk.RunSynchronously();

                //go back to the Start after downloading the stream
                target.Position = 0;
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to connect to Azure Blob Storage container '{0}'", ShortName), ex);
            }
        }

        public async Task SetFileAsync(string fileName, Stream stream)
        {
            try
            {
                var containerRef = GetBlobContainer();
                var blockBlobFileRef = containerRef.GetBlockBlobReference(fileName);

                await blockBlobFileRef.UploadFromStreamAsync(stream);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to connect to Azure Blob Storage container '{0}'", ShortName), ex);
            }
        }
        public void SetFile(string fileName, Stream stream)
        {
            try
            {
                var containerRef = GetBlobContainer();
                var blockBlobFileRef = containerRef.GetBlockBlobReference(fileName);

                blockBlobFileRef.UploadFromStreamAsync(stream).RunSynchronously();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to connect to Azure Blob Storage container '{0}'", ShortName), ex);
            }
        }

        public async Task<string> GetFileHashAsync(string fileName)
        {
            try
            {
                var containerRef = GetBlobContainer();
                var blobFileRef = await containerRef.GetBlobReferenceFromServerAsync(fileName);

                return blobFileRef.Properties.ContentMD5;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to connect to Azure Blob Storage container '{0}'", ShortName), ex);
            }
        }
        public string GetFileHash(string fileName)
        {
            try
            {
                var containerRef = GetBlobContainer();
                var blobFileRef = containerRef.GetBlobReference(fileName);

                return blobFileRef.Properties.ContentMD5;

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to connect to Azure Blob Storage container '{0}'", ShortName), ex);
            }
        }

        public async Task RemoveFileAsync(string fileName)
        {
            try
            {
                var containerRef = GetBlobContainer();
                var blobFileRef = await containerRef.GetBlobReferenceFromServerAsync(fileName);

                await blobFileRef.DeleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to connect to Azure Blob Storage container '{0}'", ShortName), ex);
            }
        }
        public void RemoveFile(string fileName)
        {
            try
            {
                var containerRef = GetBlobContainer();
                var blobFileRef = containerRef.GetBlobReference(fileName);

                blobFileRef.DeleteAsync().RunSynchronously();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to connect to Azure Blob Storage container '{0}'", ShortName), ex);
            }
        }

        public bool TestConnection()
        {
            try
            {
                var containerRef = GetBlobContainer();
                var blobFileRef = containerRef.GetBlockBlobReference("test.txt");

                var testTextString = "Test Content";

                blobFileRef.UploadTextAsync(testTextString).RunSynchronously();

                var task = blobFileRef.DownloadTextAsync();

                task.RunSynchronously();
                var testResult = task.Result;
                if (testResult == testTextString)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to connect to Azure Blob Storage container '{0}'", ShortName), ex);
            }
        }
    }
}
