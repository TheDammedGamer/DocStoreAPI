using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocStore.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DocStore.Server.Shared 
{
    public class DocStoreContextFactory : IDesignTimeDbContextFactory<DocStoreContext>
    {
        public DocStoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DocStoreContext>();
            optionsBuilder.UseSqlServer("Server=SRV2012\\LIAMDEV;Database=DocStore;User Id=liamdevsa;Password=Bradford23;");

            return new DocStoreContext(optionsBuilder.Options);
        }
    }


}
