using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WellboreProfileView.DataProvider.Interface;
using WellboreProfileView.Models.DataBaseModels;

namespace WellboreProfileView.DataProvider
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        private readonly string connectionString;

        public DbSet<Area> Areas { get; set; }

        public DbSet<Well> Wells { get; set; }

        public DbSet<Wellbore> Wellbores { get; set; }

        public DbSet<ProfilePath> ProfilePaths { get; set; }

        public DatabaseContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ApplicationException("Не задана строка подключения к БД");

            optionsBuilder.UseSqlServer(connectionString);
        }

        public DatabaseFacade GetDatabase()
        {
            return Database;
        }
    }
}
