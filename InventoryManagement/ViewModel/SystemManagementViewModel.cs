using AutoUpdate;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Service.AutoApp;
using InventoryManagement.Service.Drawing;
using InventoryManagement.Service.NetWork;
using InventoryManagement.Utils;
using InventoryManagement.Views.SystemManagement;
using Newtonsoft.Json;
using Refit;

namespace InventoryManagement.ViewModel
{
    internal partial class SystemManagementViewModel : ObservableValidator
    {
        private Action EditIpAddress;
        private Action InboundBackhaul;
        private Action UnboundBackhaul;
        private Action InventoryBackhaul;
        private Action BasicInformationDownload;
        private Action UpgradeApplication;
        public SystemManagementPage SystemManagementPage { get; set; }
        public IDictionary<string, object> keyValuePairs;
        public IDictionary<string, object> KeyValuePairs { get => keyValuePairs; set => SetProperty(ref keyValuePairs, value, true); }

        public SystemManagementViewModel()
        {
            KeyValuePairs = GetKeyValuePairs();
        }
        private void IpAddressSetting()
        {
            SystemManagementPage.Dispatcher.Dispatch(() =>
            {
                LoginViewModel loginViewModel = DependencyService.Get<LoginViewModel>();
                loginViewModel.OnOpenLoginSettingDialog();
            });
        }
        private void ToInboundBackhaul()
        {
            SystemManagementPage.Dispatcher.Dispatch(async () =>
            {
                try
                {
                    bool isUpload = await SystemManagementPage.DisplayAlert("提示", "是否进行入库回传", "确定", "取消");
                    if (!isUpload)
                        return;
                    LoadingUtils.LoadingStart("进行操作中");
                    LoginViewModel loginViewModel = DependencyService.Get<LoginViewModel>();
                    loginViewModel.GetIpAddress();
                    IList<Inbound> inbounds = SqliteHelper.Current.db.Inbounds.Where(inbound => inbound.STA == "已入库" && inbound.UPSTATE == "待回传").ToList();
                    for (int i = 0; i < inbounds.Count; i++)
                    {
                        inbounds[i].UPTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        inbounds[i].UPSTATE = "已回传";
                    }
                    IDictionary<string, object> keyValues = await NetWorkService.For<IInformationUploadService>().UploadInbound(inbounds);
                    if (keyValues["isUpload"].Equals(false))
                        throw new Exception("回传失败，具体错误请前往服务器后台查看");
                    SqliteHelper.Current.db.UpdateRange(inbounds);
                    await SqliteHelper.Current.db.SaveChangesAsync();
                    LoadingUtils.LoadingEnd();
                    await MessageUtils.ShowPopuSuccessAsync("回传成功");
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("connect"))
                        SystemManagementPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger("不能连接服务器，请确认IP是否正确"));
                    else
                        SystemManagementPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger(e.Message));
                    LoadingUtils.LoadingEnd();
                }
            });
        }
        private void ToUnboundBackhaul()
        {
            SystemManagementPage.Dispatcher.Dispatch(async () =>
            {
                try
                {
                    bool isUpload = await SystemManagementPage.DisplayAlert("提示", "是否进行入库回传", "确定", "取消");
                    if (!isUpload)
                        return;
                    LoadingUtils.LoadingStart("进行操作中");
                    LoginViewModel loginViewModel = DependencyService.Get<LoginViewModel>();
                    loginViewModel.GetIpAddress();
                    IList<Unbound> unbounds = SqliteHelper.Current.db.Unbounds.Where(unbound => unbound.STA == "已出库" && unbound.UPSTATE == "待回传").ToList();
                    for (int i = 0; i < unbounds.Count; i++)
                    {
                        unbounds[i].UPTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        unbounds[i].UPSTATE = "已回传";
                    }
                    IDictionary<string, object> keyValues = await NetWorkService.For<IInformationUploadService>().UploadUnbound(unbounds);
                    if (keyValues["isUpload"].Equals(false))
                        throw new Exception("回传失败，具体错误请前往服务器后台查看");
                    IDrawingService drawingService = DependencyService.Get<IDrawingService>();

                    DirectoryInfo directoryInfo = new DirectoryInfo(drawingService.FilePath);
                    IList<FileInfo> fileList = directoryInfo.EnumerateFiles().ToList();
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        StreamPart streamPart = new StreamPart(fileList[i].OpenRead(), fileList[i].Name);
                        string hashMap = JsonConvert.SerializeObject(
                            new Dictionary<string, string>
                            {
                                {"fileFolder", "Personnel_Credentials" }
                            });
                        await NetWorkService.For<IInformationUploadService>().UploadUnboundLLRNAMEPZ(streamPart, hashMap);
                    }
                    SqliteHelper.Current.db.UpdateRange(unbounds);
                    await SqliteHelper.Current.db.SaveChangesAsync();
                    LoadingUtils.LoadingEnd();
                    await MessageUtils.ShowPopuSuccessAsync("回传成功");
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("connect"))
                        SystemManagementPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger("不能连接服务器，请确认IP是否正确"));
                    else
                        SystemManagementPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger(e.Message));
                    LoadingUtils.LoadingEnd();
                }
            });
        }
        private void ToInventoryBackhaul()
        {
            SystemManagementPage.Dispatcher.Dispatch(async () =>
            {
                try
                {
                    bool isUpload = await SystemManagementPage.DisplayAlert("提示", "是否进行入库回传", "确定", "取消");
                    if (!isUpload)
                        return;
                    LoadingUtils.LoadingStart("进行操作中");
                    LoginViewModel loginViewModel = DependencyService.Get<LoginViewModel>();
                    loginViewModel.GetIpAddress();
                    IList<Inventory> inventories = SqliteHelper.Current.db.Inventories.Where(inventory => inventory.STA == "已盘点" && inventory.UPSTATE == "待回传").ToList();
                    for (int i = 0; i < inventories.Count; i++)
                    {
                        inventories[i].UPTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        inventories[i].UPSTATE = "已回传";
                    }
                    IDictionary<string, object> keyValues = await NetWorkService.For<IInformationUploadService>().UploadInventory(inventories);
                    if (keyValues["isUpload"].Equals(false))
                        throw new Exception("回传失败，具体错误请前往服务器后台查看");
                    SqliteHelper.Current.db.UpdateRange(inventories);
                    await SqliteHelper.Current.db.SaveChangesAsync();
                    LoadingUtils.LoadingEnd();
                    await MessageUtils.ShowPopuSuccessAsync("回传成功");
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("connect"))
                        SystemManagementPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger("不能连接服务器，请确认IP是否正确"));
                    else
                        SystemManagementPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger(e.Message));
                    LoadingUtils.LoadingEnd();
                }
            });
        }
        private void ToBasicInformationDownload()
        {
            SystemManagementPage.Dispatcher.Dispatch(async () =>
            {
                try
                {
                    bool isUpload = await SystemManagementPage.DisplayAlert("提示", "是否进行基础数据下载？", "确定", "取消");
                    if (!isUpload)
                        return;
                    LoadingUtils.LoadingStart("进行操作中");
                    LoginViewModel loginViewModel = DependencyService.Get<LoginViewModel>();
                    loginViewModel.GetIpAddress();
                    IList<Mbase> mbases = await NetWorkService.For<IInformationUploadService>().DownloadMbase();
                    for (int i = 0; i < mbases.Count; i++)
                    {
                        if (mbases[i].IVER != null)
                        {
                            Mbase m = SqliteHelper.Current.db.Mbases.Where(mbase => mbase.ICD == mbases[i].ICD && mbase.IVER == mbases[i].IVER).FirstOrDefault();
                            mbases[i].UUID = m?.UUID ?? Guid.NewGuid().ToString();
                            mbases[i].ICD = m?.ICD;
                            mbases[i].IVER = m?.IVER;
                            SqliteHelper.Current.db.AddOrUpdate(mbases[i]);
                        }
                        else
                        {
                            Mbase m = SqliteHelper.Current.db.Mbases.Where(mbase => mbase.ICD == mbases[i].ICD).FirstOrDefault();
                            mbases[i].UUID = m?.UUID ?? Guid.NewGuid().ToString();
                            mbases[i].ICD = m?.ICD;
                            SqliteHelper.Current.db.AddOrUpdate(mbases[i]);
                        }
                    }
                    await SqliteHelper.Current.db.SaveChangesAsync();
                    LoadingUtils.LoadingEnd();
                    await MessageUtils.ShowPopuWarningAsync("下载成功");
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("connect"))
                        SystemManagementPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger("不能连接服务器，请确认IP是否正确"));
                    else
                        SystemManagementPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger(e.Message));
                    LoadingUtils.LoadingEnd();
                }
            });
        }
        private async void ToUpgradeApplication()
        {
            IAutoAppService autoAppService = DependencyService.Get<IAutoAppService>();
            LoadingUtils.LoadingStart("检查更新中");
            bool isUpdate = await autoAppService.CheckAppVersionOrUpdate();
            if (!isUpdate)
                await MessageUtils.ShowPopuWarningAsync("应用没有更新");
            LoadingUtils.LoadingEnd();
        }
        public IDictionary<string, object> GetKeyValuePairs()
        {
            EditIpAddress = IpAddressSetting;
            InboundBackhaul = ToInboundBackhaul;
            UnboundBackhaul = ToUnboundBackhaul;
            InventoryBackhaul = ToInventoryBackhaul;
            BasicInformationDownload = ToBasicInformationDownload;
            UpgradeApplication = ToUpgradeApplication;
            return
            new Dictionary<string, object>()
            {
                { Guid.NewGuid().ToString(), new { Title = "IP地址设置",Required = false,ActionMethod = EditIpAddress} },
                { Guid.NewGuid().ToString(), new { Title = "入库回传",Required = false,ActionMethod = InboundBackhaul} },
                { Guid.NewGuid().ToString(), new { Title = "出库回传",Required = false,ActionMethod = UnboundBackhaul} },
                { Guid.NewGuid().ToString(), new { Title = "盘点回传",Required = false,ActionMethod = InventoryBackhaul} },
                { Guid.NewGuid().ToString(), new { Title = "基础信息下载",Required = false,ActionMethod = BasicInformationDownload} },
                { Guid.NewGuid().ToString(), new { Title = "关于应用/升级应用",Required = false,ActionMethod = UpgradeApplication} },
            };
        }
    }
}
