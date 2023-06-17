using Core.Constants;
using Core.DependencyInjection;
using Core.Settings.Concrete;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalizationConfig(this IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("tr-TR")
                };

                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                {
                    var languages = context.Request.Headers["Accept-Language"].ToString();
                    var currentLanguage = languages.Split(',').FirstOrDefault();
                    var defaultLanguage = string.IsNullOrEmpty(currentLanguage) ? "en-US" : currentLanguage;

                    if (supportedCultures.Where(x => x.Name == defaultLanguage).Count() == 0)
                        defaultLanguage = "en-US";

                    return Task.FromResult(new ProviderCultureResult(defaultLanguage, defaultLanguage));
                }));
            });

            return services;
        }

        public static IServiceCollection AddAdvancedDependencyInjection(this IServiceCollection services)
        {
            services.Scan(scan => scan
            .FromAssemblies(AssemblyContext.GetApplicationAssemblies())
            .AddClassesFromInterfaces());

            return services.AddCommonServices();
        }

        private static IImplementationTypeSelector AddClassesFromInterfaces(this IImplementationTypeSelector selector)
        {
            //singleton
            selector.AddClasses(classes => classes.AssignableTo<ISingletonLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithSingletonLifetime()

            .AddClasses(classes => classes.AssignableTo<ISelfSingletonLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithSingletonLifetime()

            //transient
            .AddClasses(classes => classes.AssignableTo<ITransientLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithTransientLifetime()

            .AddClasses(classes => classes.AssignableTo<ISelfTransientLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithTransientLifetime()

            //scoped
            .AddClasses(classes => classes.AssignableTo<IScopedLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithScopedLifetime()

            .AddClasses(classes => classes.AssignableTo<ISelfScopedLifetime>(), true)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithScopedLifetime();

            return selector;
        }

        private static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.TryAddSingleton(services);

            return services;
        }

        public static IServiceCollection AddDependencyResolvers(this IServiceCollection services,
            ICoreModule[] modules)
        {
            foreach (var module in modules)
            {
               module.Load(services); 
            }

            return ServiceTool.Create(services);
        }

        public static DbContextOptionsBuilder BuildAppDbContext(this DbContextOptionsBuilder optionsBuilder, AppSettings appSettings)
        {
            if (string.IsNullOrWhiteSpace(appSettings.ConnectionString))
                throw new InvalidOperationException("Could not find a connection string.");

            switch (appSettings.DataProvider)
            {
                case DataProvider.SQLSERVER:
                {
                    optionsBuilder.UseSqlServer(appSettings.ConnectionString, o =>
                    {
                        o.MigrationsHistoryTable("_MigrationHistory");
                        o.MigrationsAssembly("Migrations.SQLServer");
                    });
                    break;
                }
                case DataProvider.POSTGRESQL:
                {
                    optionsBuilder.UseNpgsql(appSettings.ConnectionString, o =>
                    {
                        o.MigrationsHistoryTable("_MigrationHistory");
                        o.MigrationsAssembly("Migrations.PostgreSQL");
                    });
                    break;
                }
                case DataProvider.MYSQL:
                {
                    optionsBuilder.UseMySql(appSettings.ConnectionString, ServerVersion.AutoDetect(appSettings.ConnectionString), o =>
                    {
                        o.MigrationsHistoryTable("_MigrationHistory");
                        o.MigrationsAssembly("Migrations.MySQL");
                    });
                    break;
                }
                default:
                {
                    throw new NotSupportedException($"{appSettings.DataProvider} provider doesn't support.");
                }
            }

            optionsBuilder.EnableDetailedErrors();

            return optionsBuilder;
        }
    }
}
