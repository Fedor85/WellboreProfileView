using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace WellboreProfileView.DataProvider.Interface
{
    public interface IDatabaseContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DatabaseFacade GetDatabase();

        int SaveChanges();
    }
}
