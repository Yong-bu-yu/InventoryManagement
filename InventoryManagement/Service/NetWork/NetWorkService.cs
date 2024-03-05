using InventoryManagement.DataBase;
using InventoryManagement.Model;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Service.NetWork
{
    internal static class NetWorkService
    {
        private static string BaseURL
        {
            get
            {
                return SqliteHelper.Current.db.SystemManagements.FirstOrDefault()?.IpAddress ?? throw new WebException("没有设置IP地址");
            }
        }

        public static T For<T>()
        {
            return RestService.For<T>(BaseURL);
        }
    }
}
