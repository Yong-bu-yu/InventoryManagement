using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.DataBase
{
    class SqliteHelper
    {
        static SqliteHelper baseSqlite;
        public static SqliteHelper Current => baseSqlite ??= new SqliteHelper();
        public EFCoreDbContext db;

        public SqliteHelper()
        {
            db ??= DependencyService.Get<IEFCoreDbSQLite>().Initialization();
        }

        public bool CreateOrUpdateAllTables()
        {
            return db.Database.EnsureCreated();
        }
    }
}
