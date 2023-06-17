using Core.Extensions;
using Core.Settings.Concrete;
using Core.Utilities.IoC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Migrations.SQLServer
{
    public class SQLServerContextFactory : IDesignTimeDbContextFactory<SQLServerContext>
    {
        public SQLServerContext CreateDbContext(string[] args)
        {
            var appSettings = ServiceTool.ServiceProvider.GetService<IOptionsSnapshot<AppSettings>>();

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.BuildAppDbContext(appSettings.Value);

            var context = new SQLServerContext(optionsBuilder.Options);
            return context;
        }
    }
}