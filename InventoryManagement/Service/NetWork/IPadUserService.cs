using InventoryManagement.Model;
using Refit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Service.NetWork
{
    internal interface IPadUserService
    {
        [Post("/paduser/getUserGroups")]
        Task<IList<string>> GetUserGroups();

        [Post("/paduser/getUsersByGroup")]
        [Headers("Content-Type: application/json;charset=UTF-8")]
        Task<IList<PadUser>> GetUserList([Body] IDictionary<string,string> hashMap);

        [Post("/app/getApkInfo")]
        Task<IDictionary<string,object>> GetAppInfo();
    }
}
