using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocStoreAPI.Models;
using System.IO;

namespace DocStoreAPI.Services
{
    public class DocumentService : IHostedService, IDisposable
    {
        private readonly DocStoreContext _context;
        private readonly ILogger _logger;

        public DocumentService(ILogger<DocumentService> logger, DocStoreContext docStoreContext)
        {
            _logger = logger;
            _context = docStoreContext;
        }

        public Stream GetDocument(Metadata meta)
        {
            throw new NotImplementedException();
        }

        //Returns MD5
        public string UploadDocument(Metadata meta, Stream doc)
        {
            throw new NotImplementedException();
        }

        public void MoveDocument(Metadata meta, string newStorProvider)
        {
            throw new NotImplementedException();
        }

        public void DeleteDocument(Metadata meta)
        {
            throw new NotImplementedException();
        }


        public void Dispose()
        {
            // don't need to do anything yet
            // Will need to ensure that all streams are closed
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
