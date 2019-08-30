using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocStore.API.Models;
using System.IO;
using DocStore.API.Models.Stor;
using DocStore.Shared.Models;

namespace DocStore.API.Repositories
{
    public class DocumentRepository
    {
        private readonly DocStoreContext _context;
        private readonly ILogger _logger;
        private readonly StorConfig _config; //singleton

        public DocumentRepository(ILogger<DocumentRepository> logger, DocStoreContext docStoreContext, StorConfig storConfig)
        {
            _logger = logger;
            _context = docStoreContext;
            _config = storConfig;
        }

        //Gets
        public async Task<Stream> GetDocumentAsync(MetadataEntity meta)
        {
            var stor = _config.Stors.First(s => s.ShortName == meta.StorName).GetStorFromConfig();
            var fileName = meta.GetServerFileName();
            var stream = await stor.GetFileAsync(fileName);
            return stream;
        }
        public async Task<Stream> GetDocumentByIdAsync(int id)
        {
            var meta = _context.MetadataEntities.First(me => me.Id == id);
            return await GetDocumentAsync(meta);
        }
        public async Task<Stream> GetDocumentVersionAsync(DocumentVersionEntity verEntity)
        {
            var stor = _config.Stors.First(s => s.ShortName == verEntity.StorName).GetStorFromConfig();
            var fileName = verEntity.GetServerFileName();
            var stream = await stor.GetFileAsync(fileName);
            return stream;
        }

        //Uploads all return MD5
        public async Task<string> SetDocumentAsync(MetadataEntity meta, Stream doc)
        {
            var stor = _config.Stors.First(s => s.ShortName == meta.StorName).GetStorFromConfig();
            var fileName = meta.GetServerFileName();
            await stor.SetFileAsync(fileName, doc);
            var hash = await stor.GetFileHashAsync(fileName);
            return hash;
        }
        public async Task<string> SetDocumentByIdAsync(int id, Stream doc)
        {
            var meta = _context.MetadataEntities.First(me => me.Id == id);
            return await SetDocumentAsync(meta, doc);
        }
        
        //Move Document
        public async Task MoveDocumentAsync(MetadataEntity meta, string newStorProvider)
        {
            
            try {
                var origStor = _config.Stors.FirstOrDefault(s => s.ShortName == meta.StorName).GetStorFromConfig();
                var newStor = _config.Stors.FirstOrDefault(s => s.ShortName == newStorProvider).GetStorFromConfig();
                if (String.IsNullOrWhiteSpace(origStor.ShortName))
                    throw new Exception("Unable to find old stor within config");
                if (String.IsNullOrWhiteSpace(newStor.ShortName))
                    throw new Exception("Unable to find new stor within config");
                var fileName = meta.GetServerFileName();
                string hash;
                using (var origStream = await origStor.GetFileAsync(fileName))
                {
                    await newStor.SetFileAsync(fileName, origStream);
                    hash = await newStor.GetFileHashAsync(fileName);
                    if (hash != meta.MD5Hash)
                    {
                        await newStor.RemoveFileAsync(fileName);
                        throw new Exception("File transfer failed MD5 hash is not equal");
                    }                        
                }
                await origStor.RemoveFileAsync(fileName);
            }
            catch (Exception ex) {
                throw new Exception("Unable to move document see inner excption", ex);
            }
        }
        public async Task MoveDocumentbyID(int id, string newStorProvider)
        {
            var meta = _context.MetadataEntities.First(me => me.Id == id);
            await MoveDocumentAsync(meta, newStorProvider);
        }
        public async Task MoveDocumentVersion(DocumentVersionEntity verEntity, string newStorProvider)
        {
            try
            {
                IStor origStor = _config.Stors.First(s => s.ShortName == verEntity.StorName).GetStorFromConfig();
                IStor newStor = _config.Stors.First(s => s.ShortName == newStorProvider).GetStorFromConfig();
                if (String.IsNullOrWhiteSpace(origStor.ShortName))
                    throw new Exception("Unable to find old stor within config");
                if (String.IsNullOrWhiteSpace(newStor.ShortName))
                    throw new Exception("Unable to find new stor within config");
                var fileName = verEntity.GetServerFileName();
                string hash;
                using (var origStream = await origStor.GetFileAsync(fileName))
                {
                    await newStor.SetFileAsync(fileName, origStream);
                    hash = await newStor.GetFileHashAsync(fileName);
                    if (hash != verEntity.MD5Hash)
                    {
                        await newStor.RemoveFileAsync(fileName);
                        throw new Exception("File transfer failed MD5 hash is not equal");
                    }
                }
                await origStor.RemoveFileAsync(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to move document see inner exception", ex);
            }
        }

        //Deletes
        public async Task DeleteDocumentAsync(MetadataEntity meta)
        {
            IStor stor = _config.Stors.First(s => s.ShortName == meta.StorName).GetStorFromConfig();
            var fileName = meta.GetServerFileName();
            await stor.RemoveFileAsync(fileName);
        }
        public async Task DeleteDocumentByIdAsync(int id)
        {
            var meta = _context.MetadataEntities.First(me => me.Id == id);
            await DeleteDocumentAsync(meta);
        }
        public async Task DeleteDocumentVersionAsync(DocumentVersionEntity verEntity)
        {
            IStor stor = _config.Stors.First(s => s.ShortName == verEntity.StorName).GetStorFromConfig();
            var fileName = verEntity.GetServerFileName();
            await stor.RemoveFileAsync(fileName);
        }

        //Rename
        public async Task RenameFileAsync(MetadataEntity oldFile, MetadataEntity newFile)
        {
            IStor stor = _config.Stors.First(s => s.ShortName == oldFile.StorName).GetStorFromConfig();
            var oldFileName = oldFile.GetServerFileName();
            var newFileName = newFile.GetServerFileName();
            await stor.RenameFileAsync(oldFileName, newFileName);
        }
    }
}
