using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocStore.Server.Models;
using DocStore.Server.Models.Stor;
using DocStore.Server.Repositories;
using DocStore.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DocStore.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true);
                context.Configuration = config.Build();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<DocStoreContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
                services.AddSingleton(typeof(StorConfig), StorConfigFactory.GetStorConfig(context.Configuration.GetValue<string>("StorConfigFilePath")));

                services.AddSingleton<MetadataRepository>();
                services.AddSingleton<DocumentRepository>();
                services.AddSingleton<SecurityRepository>();
                services.AddSingleton<GroupRepository>();
                services.AddSingleton<AccessRepository>();
                services.AddSingleton<BuisnessAreaRepository>();

                services.AddLogging(configure => configure.AddEventSourceLogger());

                services.AddHostedService<Worker>();
                services.AddHostedService<QueueService>();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                
                logging.AddEventLog(settings =>
                {
                    settings.LogName = "DocStore.Worker";

                });
                if (context.HostingEnvironment.IsDevelopment())
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddConsole();
                }
                else
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                }
            });
    }
}
