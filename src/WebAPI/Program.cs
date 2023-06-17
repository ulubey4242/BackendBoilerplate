using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.DependencyResolvers.Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new AutofacBusinessModule());
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    webBuilder.ConfigureAppConfiguration(x =>
                    {
                        x.SetBasePath(Environment.CurrentDirectory);
                        x.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        x.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
                        x.AddEnvironmentVariables();
                        x.Build();
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
