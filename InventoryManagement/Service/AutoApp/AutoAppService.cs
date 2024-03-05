using AutoUpdate;
using InventoryManagement.Service.NetWork;
using InventoryManagement.ViewModel;
using Microsoft.Maui.Dispatching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace InventoryManagement.Service.AutoApp
{
    internal class AutoAppService : IAutoAppService
    {

        private bool isUpdate = false;
        public bool IsAutoApp
        {
            get => IsAutoApp;
            set
            {
                if (value)
                {
                    Application.Current.MainPage.Dispatcher.DispatchAsync(async () =>
                    {
                        while (!isUpdate)
                        {
                            await CheckAppVersionOrUpdate();
                            await Task.Delay(1000);
                        }
                    });
                }
            }
        }

        public async Task<bool> CheckAppVersionOrUpdate()
        {
            try
            {
                IDictionary<string, object> appInfo = await GetAppInfo();
                if (appInfo is null)
                    return false;
                long apkVersion = (long)appInfo["APKVERSIONCODE"];
                string apkName = (string)appInfo["APKNAME"];
                string apkURL = (string)appInfo["APKURL"];
                if (apkVersion > UpdateManager.VersionCode)
                {
                    isUpdate = true;
                    LoginViewModel loginViewModel = DependencyService.Get<LoginViewModel>();
                    if (loginViewModel.IpAddress is null)
                        loginViewModel.GetIpAddress();
                    string updateAppIp = loginViewModel.IpAddress;
                    UpdatesCheckResponse updatesCheckResponse = new UpdatesCheckResponse(true, $"{updateAppIp}/file/downloadStream?fileName={apkName}&fileFolder={apkURL}");
                    UpdateManager.Clearance();
                    UpdateManagerParameters parameters = new UpdateManagerParameters
                    {
                        Title = "可更新",
                        Message = "新版本已发布。请更新！",
                        Confirm = "更新",
                        Cancel = "取消",
                        CheckForUpdatesFunction = () => Task.FromResult(updatesCheckResponse)
                    };
                    await UpdateManager
                        .Initialize(parameters, UpdateMode.AutoInstall)
                        .SetUpdateAppPage(Application.Current.MainPage)
                        .CheckUpdateAppAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public async Task<IDictionary<string, object>> GetAppInfo()
        {
            try
            {
                return await NetWorkService.For<IPadUserService>().GetAppInfo();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
