using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Service.Scanner;
using InventoryManagement.Utils;
using InventoryManagement.Views.Inbound;
using InventoryManagement.Views.Template;
using Newtonsoft.Json;
using System.Reflection;

namespace InventoryManagement.ViewModel
{
    internal partial class InboundViewModel : ObservableValidator
    {
        private Action NoMethod;
        private Action EditNumber;
        private ScanService scanService = DependencyService.Get<ScanService>();
        public bool IsDisabled { get; set; }
        public InboundPage InboundPage { get; set; }
        public Inbound CurrentInbound { get; set; }
        private List<Inbound> inboundList;
        public List<Inbound> InboundList { get => inboundList; set => SetProperty(ref inboundList, value, true); }

        public IDictionary<string, object> keyValuePairs;
        public IDictionary<string, object> KeyValuePairs { get => keyValuePairs; set => SetProperty(ref keyValuePairs, value, true); }
        public InboundViewModel()
        {
            KeyValuePairs = GetKeyValuePairs();
            InboundList = SqliteHelper.Current.db.Inbounds.Where(inbound =>
                inbound.UPSTATE != "已回传" &&
                inbound.STA != "已入库").ToList();
        }
        [RelayCommand]
        public async Task OnShowInboundAsync()
        {
            string dateSelectName = await InboundPage.DisplayActionSheet("时间选择", "取消", null, "当天", "本周", "上周", "本月", "自定义");
            if (dateSelectName != null && !dateSelectName.Equals("取消"))
            {
                (DateTime startTime, DateTime endTime) dateTimeRang = await GetDateTimeRang(dateSelectName);
                InboundList = SqliteHelper.Current.db.Inbounds.Where(inbound =>
                dateTimeRang.startTime.CompareTo((DateTime)(object)inbound.CTI) <= 0 && dateTimeRang.endTime.CompareTo((DateTime)(object)inbound.CTI) >= 0 &&
                inbound.UPSTATE != "已回传" &&
                inbound.STA == "已入库"
                )
                    .OrderByDescending(inbound => inbound.CTI)
                    .ThenByDescending(inbound => inbound.ETI)
                    .ToList();
            }
        }
        [RelayCommand]
        public async Task OnHideInboundAsync()
        {
            (DateTime startTime, DateTime endTime) dateTimeRang = await GetDateTimeRang("当天");
            InboundList = SqliteHelper.Current.db.Inbounds.Where(inbound =>
            dateTimeRang.startTime.CompareTo((DateTime)(object)inbound.CTI) <= 0 && dateTimeRang.endTime.CompareTo((DateTime)(object)inbound.CTI) >= 0 &&
            inbound.UPSTATE != "已回传" &&
            inbound.STA != "已入库"
            )
                .OrderByDescending(inbound => inbound.CTI)
                .ThenByDescending(inbound => inbound.ETI)
                .ToList();
        }
        [RelayCommand]
        public async Task OnAddInboundAsync()
        {
            IsDisabled = true;
            CurrentInbound = new Inbound()
            {
                STA = "待入库",
                UPSTATE = "待回传",
                CTI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            KeyValuePairs = GetKeyValuePairs();
            scanService.Scan += ScanService_Scan;
            Shell.Current.Navigating += Current_Navigating;
            await Shell.Current.Navigation.PushAsync(new InboundMoreActionsPage(), true);
            await Snackbar.Make("请按扫描键扫描对应的二维码", null, "知道了", TimeSpan.FromSeconds(5)).Show();
        }
        [RelayCommand(CanExecute = "IsDisabled")]
        public async Task OnSaveInboundAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }
        [RelayCommand]
        public async Task OnEditInboundAsync(Inbound inbound)
        {
            IsDisabled = true;
            CurrentInbound = inbound;
            KeyValuePairs = GetKeyValuePairs();
            scanService.Scan += ScanService_Scan;
            Shell.Current.Navigating += Current_Navigating;
            await Shell.Current.Navigation.PushAsync(new InboundMoreActionsPage(), true);
        }
        [RelayCommand]
        public async Task OnDeleteInboundAsync(Inbound inbound)
        {
            IsDisabled = false;
            bool isDelete = await InboundPage.DisplayAlert("警告", "你是否删除此条数据，该操作不可撤销？", "确认", "取消");
            if (isDelete)
            {
                SqliteHelper.Current.db.Remove(inbound);
                await SqliteHelper.Current.db.SaveChangesAsync();
                InboundList = SqliteHelper.Current.db.Inbounds.Where(inbound =>
                inbound.UPSTATE != "已回传" &&
                inbound.STA != "已入库").ToList();
            }
        }
        [RelayCommand]
        public async Task OnGetInfoInboundAsync(Inbound inbound)
        {
            IsDisabled = false;
            CurrentInbound = inbound;
            KeyValuePairs = GetKeyValuePairs();
            await Shell.Current.Navigation.PushAsync(new InboundMoreActionsPage(), true);
        }
        private async void Current_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.Source == ShellNavigationSource.PopToRoot && IsDisabled)
            {
                ShellNavigatingDeferral token = e.GetDeferral();
                if (string.IsNullOrEmpty(CurrentInbound.RNO))
                {
                    token.Complete();
                    scanService.Scan -= ScanService_Scan;
                    Shell.Current.Navigating -= Current_Navigating;
                    return;
                }
                bool isSave = await InboundPage.DisplayAlert("信息", "是否保存当前数据？", "确认", "取消");
                if (isSave)
                {
                    bool isStaChange = false;
                    foreach (KeyValuePair<string, object> keyValue in KeyValuePairs)
                    {
                        bool required = (bool)keyValue.Value.GetObjectPropertyValue("Required");
                        object inboundValue = CurrentInbound.GetObjectPropertyValue(keyValue.Key);
                        if (required && inboundValue == null)
                        {
                            isStaChange = true;
                        }
                    }
                    if (!isStaChange)
                    {
                        CurrentInbound.STA = "已入库";
                        LoginViewModel loginViewModel = DependencyService.Get<LoginViewModel>();
                        PadUser user = loginViewModel.CurrentUser;
                        CurrentInbound.EMP = user.CN_LOGIN;
                        CurrentInbound.ETI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        Mdetail mdetail = new Mdetail();
                        Type type = CurrentInbound.GetType();
                        PropertyInfo[] propertyInfos = type.GetProperties();
                        for (int i = 0; i < propertyInfos.Length; i++)
                        {
                            mdetail.TrySetObjectPropertyValue(propertyInfos[i].Name, CurrentInbound.GetObjectPropertyValue(propertyInfos[i].Name));
                        }
                        mdetail.UUID = Guid.NewGuid().ToString();
                        mdetail.QUA = CurrentInbound.QUA;
                        mdetail.RTI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        SqliteHelper.Current.db.Add(mdetail);
                        SqliteHelper.Current.db.SaveChanges();
                    }
                    //Inbound inbound = SqliteHelper.Current.db.Inbounds.Find(CurrentInbound.RNO);
                    //if (inbound is not null)
                    //    SqliteHelper.Current.db.Add(CurrentInbound);
                    //else
                    SqliteHelper.Current.db.AddOrUpdate(CurrentInbound);
                    SqliteHelper.Current.db.SaveChanges();
                    InboundList = SqliteHelper.Current.db.Inbounds.Where(inbound =>
                inbound.UPSTATE != "已回传" &&
                inbound.STA != "已入库").ToList();
                }
                token.Complete();
                scanService.Scan -= ScanService_Scan;
                Shell.Current.Navigating -= Current_Navigating;
            }
        }

        private void ScanService_Scan(object sender, ScanEventArgs e)
        {
            try
            {
                IList<IDictionary<string, string>> keyValueList = JsonConvert.DeserializeObject<IList<IDictionary<string, string>>>(e.ScanContext);
                IDictionary<string, string> keyValuePairs = keyValueList[0];
                PropertyInfo[] propertyInfos = CurrentInbound.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (CurrentInbound.GetObjectPropertyValue(propertyInfo.Name) == null && keyValuePairs.TryGetValue(propertyInfo.Name, out string value))
                        CurrentInbound.SetObjectPropertyValue(propertyInfo.Name, value);
                }
                if (CurrentInbound.ICD is not null)
                {
                    Mbase mbase = SqliteHelper.Current.db.Mbases?.Where(mb => mb.ICD == CurrentInbound.ICD).FirstOrDefault();
                    if (mbase is not null)
                    {
                        CurrentInbound.INA = mbase.INA;
                        CurrentInbound.UT = mbase.UT;
                        CurrentInbound.ISJ = mbase.ISJ;
                    }
                }
                KeyValuePairs = GetKeyValuePairs();
            }
            catch (Exception ex)
            {
                MessageUtils.ShowPopuDanger(ex.Message);
            }
        }

        private void EditFQUA()
        {
            InboundPage.Dispatcher.Dispatch(async () =>
            {
                try
                {
                    CurrentInbound.FQUA = await InboundPage.DisplayPromptAsync("申请数据", "请输入申请数量", "确认", "取消", "请输入", -1, Keyboard.Numeric, CurrentInbound.FQUA) ?? throw new NullReferenceException("必须填写申请数量");
                    if (string.IsNullOrWhiteSpace(CurrentInbound.FQUA))
                        throw new NullReferenceException("必须填写申请数量");
                    KeyValuePairs = GetKeyValuePairs();
                }
                catch (Exception e)
                {
                    await MessageUtils.ShowPopuDangerAsync(e.Message);
                }
            });
        }

        public IDictionary<string, object> GetKeyValuePairs()
        {
            EditNumber = EditFQUA;
            return
            new Dictionary<string, object>()
            {
                { "RNO", new { Title = "入库流水号：",Required = true,ActionMethod = NoMethod} },
                { "ICD", new { Title = "物料编码：",Required = false,ActionMethod = NoMethod} },
                { "QUA", new { Title = "申请数量：",Required = false,ActionMethod = NoMethod} },
                { "FQUA", new { Title = "实收数量：",Required = true,ActionMethod = EditNumber} },
                { "UT", new { Title = "计量单位：",Required = false,ActionMethod = NoMethod} },
                { "KWCODE", new { Title = "库位：",Required = true,ActionMethod = NoMethod} },
            };
        }
        private async Task<(DateTime startTime, DateTime endTime)> GetDateTimeRang(string datetime)
        {
            DateTime st = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime et = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            switch (datetime)
            {
                case "当天":
                    break;
                case "本周":
                    st = st.AddDays(-7);
                    break;
                case "上周":
                    st = st.AddDays(-14);
                    et = et.AddDays(-7);
                    break;
                case "本月":
                    st = st.AddMonths(-1);
                    break;
                case "自定义":
                    (DateTime StartDate, DateTime EndDate)? dateRang = await InboundPage.ShowPopupAsync(new DateRangPage()) as (DateTime StartDate, DateTime EndDate)?;
                    if (dateRang != null)
                    {
                        st = new DateTime(dateRang.Value.StartDate.Year, dateRang.Value.StartDate.Month, dateRang.Value.StartDate.Day, 0, 0, 0);
                        et = new DateTime(dateRang.Value.EndDate.Year, dateRang.Value.EndDate.Month, dateRang.Value.EndDate.Day, 23, 59, 59);
                    }
                    break;
                default:
                    st = DateTime.MinValue;
                    et = DateTime.MaxValue;
                    break;
            }
            return await Task.FromResult((st, et));
        }
    }
}
