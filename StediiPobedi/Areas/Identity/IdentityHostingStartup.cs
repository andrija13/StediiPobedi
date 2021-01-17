using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StediiPobedi.Areas.Identity.Data;
using StediiPobedi.Data;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StediiPobedi.Areas.Identity.Data;
using Aguacongas.Identity.Redis;
using System.IO;

[assembly: HostingStartup(typeof(StediiPobedi.Areas.Identity.IdentityHostingStartup))]
namespace StediiPobedi.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {

            builder.ConfigureServices((services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddDebug()
                        .AddConsole()
                        .SetMinimumLevel(LogLevel.Warning)
                        .AddFilter("Aguacongas.Identity.Redis", LogLevel.Trace);
                });
            });

        }
    }
}