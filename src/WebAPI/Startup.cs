using Business.DependencyResolvers.Autofac;
using Business.Utilities.Security.Jwt;
using Core.DependencyResolvers;
using Core.Extensions;
using Core.Settings.Concrete;
using Core.Utilities.IoC;
using Core.Utilities.Security.Jwt;
using DataAccess.DependencyResolvers;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Net;
using WebAPI.Filters;
using WebAPI.Infrastructure;
using System.Data;
using System;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //configure settings
            var appSettingsSection = Configuration.GetSection("AppSettings");

            services.Configure<AppSettings>(appSettingsSection);
            services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));

            var appSettings = appSettingsSection.Get<AppSettings>();

            //add localization
            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
            services.AddLocalizationConfig();

            //add swagger
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackendBoilerplate", Version = "v1" });
                c.DocumentFilter<SwaggerIgnoreFilter>();
            });

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                    builder => builder.AllowAnyOrigin());
            });


            //add advanced dependency injection
            services.AddAdvancedDependencyInjection();

            services.AddDependencyResolvers(new ICoreModule[]
            {
                new CoreModule(),
                new DataModule(),
                new AutofacBusinessModule()
            });

            //add hangfire
            services.AddHangfire(x =>
            {
                switch (appSettings.DataProvider)
                {
                    case Core.Constants.DataProvider.SQLSERVER:
                    x.UseSqlServerStorage(appSettings.ConnectionString);
                    break;

                    case Core.Constants.DataProvider.POSTGRESQL:
                    x.UsePostgreSqlStorage(appSettings.ConnectionString);
                    break;

                    case Core.Constants.DataProvider.MYSQL:
                    x.UseStorage(new MySqlStorage(appSettings.ConnectionString,
                        new MySqlStorageOptions
                        {
                            TransactionIsolationLevel = (System.Transactions.IsolationLevel?)IsolationLevel.ReadCommitted,
                            QueuePollInterval = TimeSpan.FromSeconds(15),
                            JobExpirationCheckInterval = TimeSpan.FromHours(1),
                            CountersAggregateInterval = TimeSpan.FromMinutes(5),
                            PrepareSchemaIfNecessary = true,
                            DashboardJobListLimit = 50000,
                            TransactionTimeout = TimeSpan.FromMinutes(1),
                            TablesPrefix = "Hangfire_"
                        }
                    ));
                    break;

                    default:
                    break;
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //initialize database
            app.InitializeDatabase();

            app.ConfigureCustomExceptionMiddleware();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "text/plain"
            });

            //use localization
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions.Value);

            //use swagger
            app.UseReDoc();
            app.UseSwagger();

            //401 redirect to Login page
            var loginPage = new PathString("/Login");

            app.UseStatusCodePages(context =>
            {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
                    response.Redirect(loginPage);

                return System.Threading.Tasks.Task.CompletedTask;
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });

            var scope = app.ApplicationServices.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
            var tokenHelper = serviceProvider.GetRequiredService<ITokenHelper>();

            //use hangfire
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                IsReadOnlyFunc = (DashboardContext context) => false,
                Authorization = new[] { new HangfireAuthFilter(tokenHelper) }
            });

            switch (appSettings.Value.DataProvider)
            {
                case Core.Constants.DataProvider.SQLSERVER:
                GlobalConfiguration.Configuration.UseSqlServerStorage(appSettings.Value.ConnectionString);
                break;

                case Core.Constants.DataProvider.POSTGRESQL:
                GlobalConfiguration.Configuration.UsePostgreSqlStorage(appSettings.Value.ConnectionString);
                break;

                case Core.Constants.DataProvider.MYSQL:
                GlobalConfiguration.Configuration.UseStorage(new MySqlStorage(appSettings.Value.ConnectionString, 
                    new MySqlStorageOptions
                    {
                        TransactionIsolationLevel = (System.Transactions.IsolationLevel?)IsolationLevel.ReadCommitted,
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 50000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire_"
                    }
                 ));
                break;

                default:
                break;
            }

            _ = app.UseHangfireServer();
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 7 });
        }
    }
}
