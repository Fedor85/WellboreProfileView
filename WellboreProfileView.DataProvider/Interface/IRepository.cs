using System.Collections.Generic;
using System.Linq;

namespace WellboreProfileView.DataProvider.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAllAsQueryable();

        IQueryable<TEntity> GetAllAsNoTrackingQueryable();


        void Remove(TEntity entity);


        void RemoveRange(IEnumerable<TEntity> entitys);


        void Update(TEntity entity);


        void UpdateRange(IEnumerable<TEntity> entitys);
    }
}
