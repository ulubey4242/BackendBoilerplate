using Core.DependencyInjection;
using DataAccess.Concrete.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Migrations.MySQL
{
    public class MySQLContext : AppDataContext, ISelfScopedLifetime
    {
        public MySQLContext(DbContextOptions options) : base(options)
        {
        }
    }
}