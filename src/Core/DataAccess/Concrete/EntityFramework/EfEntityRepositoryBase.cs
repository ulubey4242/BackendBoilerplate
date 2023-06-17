using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Core.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
    where TEntity : class, IEntity, new()
    where TContext : DbContext, new()
    {
        public void Add(TEntity entity)
        {
            var context = new TContext();
            context.Add(entity);
            context.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            var context = new TContext();
            context.Remove(entity);
            context.SaveChanges();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            var context  = new TContext();
            return context.Set<TEntity>().SingleOrDefault(filter);
        }

        public IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            var context  = new TContext();

            return filter == null
                ? context.Set<TEntity>().AsQueryable()
                : context.Set<TEntity>().Where(filter).AsQueryable();
        }

        public void Update(TEntity entity)
        {
            var context  = new TContext();
            context.Update(entity);
            context.SaveChanges();
        }
    }
}
