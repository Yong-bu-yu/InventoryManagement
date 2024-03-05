using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Service.Scanner;
using InventoryManagement.Utils;
using InventoryManagement.Views.Inventory;
using InventoryManagement.Views.Template;
using Newtonsoft.Json;
using System.Reflection;

namespace InventoryManagement.ViewModel
{
    internal partial class InventoryViewModel : ObservableValidator
    {
        private Action NoMethod;
        private Action EditNumber;
        private ScanService scanService = DependencyService.Get<ScanService>();
        public bool IsDisabled { get; set; }
        public InventoryPage InventoryPage { get; set; }
        public Inventory CurrentInventory { get; set; }
        private List<Inventory> inventoryList;
        public List<Inventory> InventoryList { get => inventoryList; set => SetProperty(ref inventoryList, value, true); }

        public IDictionary<string, object> keyValuePairs;
        public IDictionary<string, object> KeyValuePairs { get => keyValuePairs; set => SetProperty(ref keyValuePairs, value, true); }
        public InventoryViewModel()
        {
            KeyValuePairs = GetKeyValuePairs();
            InventoryList = SqliteHelper.Current.db.Inventories.Where(inventory =>
                inventory.UPSTATE != "已回传" &&
                inventory.STA != "已盘点").ToList();
        }
        [RelayCommand]
        public async Task OnShowInventoryAsync()
        {
            string dateSelectName = await InventoryPage.DisplayActionSheet("时间选择", "取消", null, "当天", "本周", "上周", "本月", "自定义");
            if (dateSelectName != null && !dateSelectName.Equals("取消"))
            {
                (DateTime startTime, DateTime endTime) dateTimeRang = await GetDateTimeRang(dateSelectName);
                InventoryList = SqliteHelper.Current.db.Inventories.Where(inventory =>
                dateTimeRang.startTime.CompareTo((DateTime)(object)inventory.CTI) <= 0 && dateTimeRang.endTime.CompareTo((DateTime)(object)inventory.CTI) >= 0 &&
                inventory.UPSTATE != "已回传" &&
                inventory.STA == "已盘点"
                )
                    .OrderByDescending(inventory => inventory.CTI)
                    .OrderByDescending(inventory => inventory.ETI)
                    .ToList();
            }
        }
        [RelayCommand]
        public async Task OnHideInventoryAsync()
        {
            (DateTime startTime, DateTime endTime) dateTimeRang = await GetDateTimeRang("当天");
            InventoryList = SqliteHelper.Current.db.Inventories.Where(inventory =>
            dateTimeRang.startTime.CompareTo((DateTime)(object)inventory.CTI) <= 0 && dateTimeRang.endTime.CompareTo((DateTime)(object)inventory.CTI) >= 0 &&
            inventory.UPSTATE != "已回传" &&
            inventory.STA != "已盘点"
            )
                .OrderByDescending(inventory => inventory.CTI)
                .OrderByDescending(inventory => inventory.ETI)
                .ToList();
        }
        [RelayCommand]
        public async Task OnAddInventoryAsync()
        {
            IsDisabled = true;
            CurrentInventory = new Inventory()
            {
                STA = "待盘点",
                UPSTATE = "待回传",
                CTI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            KeyValuePairs = GetKeyValuePairs();
            scanService.Scan += ScanService_Scan;
            Shell.Current.Navigating += Current_Navigating;
            await Shell.Current.Navigation.PushAsync(new InventoryMoreActionsPage(), true);
            await Snackbar.Make("请按扫描键扫描对应的二维码", null, "知道了", TimeSpan.FromSeconds(5)).Show();
        }
        [RelayCommand(CanExecute = "IsDisabled")]
        public async Task OnSaveInventoryAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }
        [RelayCommand]
        public async Task OnEditInventoryAsync(Inventory inventory)
        {
            IsDisabled = true;
            CurrentInventory = inventory;
            KeyValuePairs = GetKeyValuePairs();
            scanService.Scan += ScanService_Scan;
            Shell.Current.Navigating += Current_Navigating;
            await Shell.Current.Navigation.PushAsync(new InventoryMoreActionsPage(), true);
        }
        [RelayCommand]
        public async Task OnDeleteInventoryAsync(Inventory inventory)
        {
            IsDisabled = false;
            bool isDelete = await InventoryPage.DisplayAlert("警告", "你是否删除此条数据，该操作不可撤销？", "确认", "取消");
            if (isDelete)
            {
                SqliteHelper.Current.db.Remove(inventory);
                await SqliteHelper.Current.db.SaveChangesAsync();
                InventoryList = SqliteHelper.Current.db.Inventories.Where(inventory =>
                inventory.UPSTATE != "已回传" &&
                inventory.STA != "已盘点").ToList();
            }
        }
        [RelayCommand]
        public async Task OnGetInfoInventoryAsync(Inventory inventory)
        {
            IsDisabled = false;
            CurrentInventory = inventory;
            KeyValuePairs = GetKeyValuePairs();
            await Shell.Current.Navigation.PushAsync(new InventoryMoreActionsPage(), true);
        }
        private async void Current_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.Source == ShellNavigationSource.PopToRoot && IsDisabled)
            {
                ShellNavigatingDeferral token = e.GetDeferral();
                if (string.IsNullOrEmpty(CurrentInventory.RNO) || string.IsNullOrEmpty(CurrentInventory.PDNO) || string.IsNullOrEmpty(CurrentInventory.KWCODE))
                {
                    token.Complete();
                    scanService.Scan -= ScanService_Scan;
                    Shell.Current.Navigating -= Current_Navigating;
                    return;
                }
                bool isSave = await InventoryPage.DisplayAlert("信息", "是否保存当前数据？", "确认", "取消");
                if (isSave)
                {
                    bool isStaChange = false;
                    foreach (KeyValuePair<string, object> keyValue in KeyValuePairs)
                    {
                        bool required = (bool)keyValue.Value.GetObjectPropertyValue("Required");
                        object inventoryValue = CurrentInventory.GetObjectPropertyValue(keyValue.Key);
                        if (required && inventoryValue == null)
                        {
                            isStaChange = true;
                            break;
                        }
                    }
                    if (!isStaChange)
                    {
                        CurrentInventory.STA = "已盘点";
                        LoginViewModel loginViewModel = DependencyService.Get<LoginViewModel>();
                        PadUser user = loginViewModel.CurrentUser;
                        CurrentInventory.EMP = user.CN_LOGIN;
                        CurrentInventory.ETI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        CurrentInventory.PTI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        Mdetail mdetail = SqliteHelper.Current.db.Mdetails.Where(mdetail => mdetail.RNO == CurrentInventory.RNO).FirstOrDefault();
                        if (mdetail is not null)
                        {
                            Type type = CurrentInventory.GetType();
                            PropertyInfo[] propertyInfos = type.GetProperties();
                            for (int i = 0; i < propertyInfos.Length; i++)
                            {
                                mdetail.TrySetObjectPropertyValue(propertyInfos[i].Name, CurrentInventory.GetObjectPropertyValue(propertyInfos[i].Name));
                            }
                            mdetail.QUA = CurrentInventory.PQUA;
                            mdetail.PDTI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            SqliteHelper.Current.db.Update(mdetail);
                            SqliteHelper.Current.db.SaveChanges();
                        }
                    }
                    //Inventory inventory = SqliteHelper.Current.db.Inventories.Find(CurrentInventory.RNO, CurrentInventory.PDNO, CurrentInventory.KWCODE);
                    //if (inventory is not null)
                    //    SqliteHelper.Current.db.Add(CurrentInventory);
                    //else
                    SqliteHelper.Current.db.AddOrUpdate(CurrentInventory);
                    SqliteHelper.Current.db.SaveChanges();
                    InventoryList = SqliteHelper.Current.db.Inventories.Where(inventory =>
                inventory.UPSTATE != "已回传" &&
                inventory.STA != "已盘点").ToList();
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
                PropertyInfo[] propertyInfos = CurrentInventory.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (CurrentInventory.GetObjectPropertyValue(propertyInfo.Name) == null && keyValuePairs.TryGetValue(propertyInfo.Name, out string value))
                        CurrentInventory.SetObjectPropertyValue(propertyInfo.Name, value);
                }
                if (CurrentInventory.ICD is not null)
                {
                    Mbase mbase = SqliteHelper.Current.db.Mbases?.Where(mb => mb.ICD == CurrentInventory.ICD).FirstOrDefault();
                    if (mbase is not null)
                    {
                        CurrentInventory.INA = mbase.INA;
                        CurrentInventory.UT = mbase.UT;
                        CurrentInventory.ISJ = mbase.ISJ;
                    }
                }
                KeyValuePairs = GetKeyValuePairs();
            }
            catch (Exception ex)
            {
                MessageUtils.ShowPopuDanger(ex.Message);
            }
        }

        private void EditPQUA()
        {
            InventoryPage.Dispatcher.Dispatch(async () =>
            {
                try
                {
                    CurrentInventory.PQUA = await InventoryPage.DisplayPromptAsync("申请数据", "请输入盘点数量", "确认", "取消", "请输入", -1, Keyboard.Numeric, CurrentInventory.FQUA) ?? throw new NullReferenceException("必须填写盘点数量");
                    if (string.IsNullOrWhiteSpace(CurrentInventory.PQUA))
                        throw new NullReferenceException("必须填写盘点数量");
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
            EditNumber = EditPQUA;
            return
            new Dictionary<string, object>()
            {
                { "RNO", new { Title = "物资编号：",Required = true,ActionMethod = NoMethod} },
                { "ICD", new { Title = "物料编码：",Required = false,ActionMethod = NoMethod} },
                { "UT", new { Title = "计量单位：",Required = false,ActionMethod = NoMethod} },
                { "PCD", new { Title = "产品编号：",Required = false,ActionMethod = NoMethod} },
                { "PQUA", new { Title = "盘点数量：",Required = true,ActionMethod = EditNumber} },
                { "PTI", new { Title = "盘点时间：",Required = false,ActionMethod = NoMethod} },
                { "KWCODE", new { Title = "盘点库位：",Required = false,ActionMethod = NoMethod} },
                { "PDNO", new { Title = "盘点批次流水号：",Required = true,ActionMethod = NoMethod} },
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
                    (DateTime StartDate, DateTime EndDate)? dateRang = await InventoryPage.ShowPopupAsync(new DateRangPage()) as (DateTime StartDate, DateTime EndDate)?;
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
