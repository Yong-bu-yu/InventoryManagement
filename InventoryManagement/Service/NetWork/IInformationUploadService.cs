using InventoryManagement.Model;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Service.NetWork
{
    internal interface IInformationUploadService
    {
        [Post("/paduser/setInbound")]
        [Headers("Content-Type: application/json;charset=UTF-8")]
        Task<IDictionary<string, object>> UploadInbound([Body] IList<Inbound> listData);

        [Post("/paduser/setUnbound")]
        [Headers("Content-Type: application/json;charset=UTF-8")]
        Task<IDictionary<string, object>> UploadUnbound([Body] IList<Unbound> listData);

        [Multipart]
        [Post("/file/upload")]
        Task UploadUnboundLLRNAMEPZ(StreamPart multipartFile, string hashMap);

        [Post("/paduser/setInventory")]
        [Headers("Content-Type: application/json;charset=UTF-8")]
        Task<IDictionary<string, object>> UploadInventory([Body] IList<Inventory> listData);

        [Post("/download/getMbase")]
        [Headers("Content-Type: application/json;charset=UTF-8")]
        Task<IList<Mbase>> DownloadMbase();
    }
}
