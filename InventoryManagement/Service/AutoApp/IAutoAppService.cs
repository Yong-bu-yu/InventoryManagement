using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Service.AutoApp
{
    internal interface IAutoAppService
    {
        bool IsAutoApp { get; set; }

        Task<IDictionary<string, object>> GetAppInfo();

        Task<bool> CheckAppVersionOrUpdate();
    }
}
