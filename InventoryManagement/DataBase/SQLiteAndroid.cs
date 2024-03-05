using InventoryManagement.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SQLiteAndroid))]//注入SQLiteAndroid
namespace InventoryManagement.DataBase
{
    class SQLiteAndroid : IEFCoreDbSQLite
    {
        private static EFCoreDbContext context;

        private static readonly object locker = new object();

        public EFCoreDbContext Initialization()
        {
            lock (locker)
            {
                if (context == null)
                {
                    context = new EFCoreDbContext();
                }
            }
            return context;
        }
    }
}
