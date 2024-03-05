using InventoryManagement.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.DataBase
{
    class EFCoreDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
#if WINDOWS
            connectionStringBuilder.DataSource = "InventoryManagement.db3";
            optionsBuilder.UseSqlite(connectionStringBuilder.ToString());
#elif ANDROID
            string androidDataBaseDirectory = FileSystem.AppDataDirectory.Replace("files", "database");
            if (!Directory.Exists(androidDataBaseDirectory))
            {
                Directory.CreateDirectory(androidDataBaseDirectory);
            }
            connectionStringBuilder.DataSource = $"{androidDataBaseDirectory}/InventoryManagement.db";
            optionsBuilder.UseSqlite(connectionStringBuilder.ToString());
#endif
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventory>()
                .HasKey(c => new { c.RNO, c.PDNO, c.KWCODE });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Inbound> Inbounds { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Mbase> Mbases { get; set; }
        public DbSet<Mdetail> Mdetails { get; set; }
        public DbSet<Unbound> Unbounds { get; set; }
        public DbSet<PadUser> PadUsers { get; set; }
        public DbSet<SystemManagement> SystemManagements { get; set; }

        public virtual EntityEntry<TEntity> AddOrUpdate<TEntity>(TEntity entity) where TEntity : class
        {
            EntityEntry<TEntity> entityEntry = Entry(entity);
            switch (entityEntry.State)
            {
                case EntityState.Detached:
                    return Add(entity);
                case EntityState.Modified:
                case EntityState.Unchanged:
                    return Update(entity);
                case EntityState.Added:
                    return Add(entity);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public virtual async Task<EntityEntry<TEntity>> AddOrUpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            EntityEntry<TEntity> entityEntry = Entry(entity);
            switch (entityEntry.State)
            {
                case EntityState.Detached:
                    return await AddAsync(entity);
                case EntityState.Modified:
                    return await Task.Run(() => Update(entity));
                case EntityState.Added:
                    return await AddAsync(entity);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
