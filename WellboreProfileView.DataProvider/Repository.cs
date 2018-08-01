using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WellboreProfileView.DataProvider.Interface;

namespace WellboreProfileView.DataProvider
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private IDatabaseContext databaseContext { get; set; }


        public Repository(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public virtual IQueryable<TEntity> GetAllAsQueryable()
        {
            return databaseContext.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAllAsNoTrackingQueryable()
        {
            return databaseContext.Set<TEntity>().AsNoTracking();
        }

        public virtual void Remove(TEntity entity)
        {
            databaseContext.Set<TEntity>().Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entitys)
        {
            databaseContext.Set<TEntity>().RemoveRange(entitys);
        }

        public virtual void Update(TEntity entity)
        {
            databaseContext.Set<TEntity>().Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entitys)
        {
            databaseContext.Set<TEntity>().UpdateRange(entitys);
        }
    }
}
