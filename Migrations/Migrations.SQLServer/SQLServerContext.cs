using Core.DependencyInjection;
using DataAccess.Concrete.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Migrations.SQLServer
{
    public class SQLServerContext : AppDataContext, ISelfScopedLifetime
    {
        public SQLServerContext(DbContextOptions options) : base(options)
        {
        }
    }
}