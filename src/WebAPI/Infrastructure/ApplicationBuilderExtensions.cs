using Core.Constants;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Settings.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;
using Entities.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Migrations.PostgreSQL;
using Migrations.SQLServer;
using Migrations.MySQL;
using System;
using System.Linq;

namespace WebAPI.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            using (var context = CreateAppDataContext(scope.ServiceProvider))
            {
                try
                {
                    //apply migrations
                    context.Database.Migrate();
                }
                catch { }

                //create test data
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (environmentName == "Development")
                    SeedData(context);
            }

            return app;
        }

        private static void SeedData(AppDataContext dataContext)
        {
            var adminRole = dataContext.OperationClaims
                                .Where(x => x.RowGuid == new Guid(Roles.Admin.Identifier()))
                                .FirstOrDefault();

            if (adminRole == null)
            {
                adminRole = new OperationClaim
                {
                    Name = Roles.Admin.Description(),
                    RowGuid = new Guid(Roles.Admin.Identifier())
                };

                dataContext.OperationClaims.Add(adminRole);
                dataContext.SaveChanges();
            }

            foreach (var testUser in TestList.Users())
            {
                var user = dataContext.Users
                                .Where(x => x.RowGuid == testUser.RowGuid)
                                .FirstOrDefault();

                if (user == null)
                {
                    user = testUser;

                    dataContext.Users.Add(user);
                    dataContext.SaveChanges();
                }

                var role = dataContext.UserOperationClaims
                                    .Where(x => x.UserId == user.Id && x.OperationClaimId == adminRole.Id)
                                    .FirstOrDefault();

                if (role == null)
                {
                    dataContext.UserOperationClaims.Add(new UserOperationClaim
                    {
                        UserId = user.Id,
                        OperationClaimId = adminRole.Id
                    });
                    dataContext.SaveChanges();
                }
            }
        }

        private static AppDataContext CreateAppDataContext(IServiceProvider serviceProvider)
        {
            var appSettings = serviceProvider.GetService<IOptionsSnapshot<AppSettings>>();

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.BuildAppDbContext(appSettings.Value);

            switch (appSettings.Value.DataProvider)
            {
                case DataProvider.SQLSERVER:
                    return new SQLServerContext(optionsBuilder.Options);
                case DataProvider.POSTGRESQL:
                    return new PostgreSQLContext(optionsBuilder.Options);
                case DataProvider.MYSQL:
                    return new MySQLContext(optionsBuilder.Options);
                default:
                {
                    throw new NotSupportedException($"{appSettings.Value.DataProvider} provider doesn't support.");
                }
            }
        }
    }
}
