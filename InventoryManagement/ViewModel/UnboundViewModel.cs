using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Service.Drawing;
using InventoryManagement.Service.Scanner;
using InventoryManagement.Utils;
using InventoryManagement.Views.Template;
using InventoryManagement.Views.Unbound;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace InventoryManagement.ViewModel
{
    internal partial class UnboundViewModel : ObservableValidator
    {
        private Action NoMethod;
        private Action EditNumber;
        private Action EditDraw;
        private ScanService scanService = DependencyService.Get<ScanService>();
        public bool IsDisabled { get; set; }
        public UnboundPage UnboundPage { get; set; }
        public Unbound CurrentUnbound { get; set; }
        private List<Unbound> unboundList;
        public List<Unbound> UnboundList { get => unboundList; set => SetProperty(ref unboundList, value, true); }

        public IDictionary<string, object> keyValuePairs;
        public IDictionary<string, object> KeyValuePairs { get => keyValuePairs; set => SetProperty(ref keyValuePairs, value, true); }
        public UnboundViewModel()
        {
            KeyValuePairs = GetKeyValuePairs();
            UnboundList = SqliteHelper.Current.db.Unbounds.Where(unbound =>
                unbound.UPSTATE != "已回传" &&
                unbound.STA != "已出库").ToList();
        }
        [RelayCommand]
        public async Task OnShowUnboundAsync()
        {
            string dateSelectName = await UnboundPage.DisplayActionSheet("时间选择", "取消", null, "当天", "本周", "上周", "本月", "自定义");
            if (dateSelectName != null && !dateSelectName.Equals("取消"))
            {
                (DateTime startTime, DateTime endTime) dateTimeRang = await GetDateTimeRang(dateSelectName);
                UnboundList = SqliteHelper.Current.db.Unbounds.Where(unbound =>
                dateTimeRang.startTime.CompareTo((DateTime)(object)unbound.CTI) <= 0 && dateTimeRang.endTime.CompareTo((DateTime)(object)unbound.CTI) >= 0 &&
                unbound.UPSTATE != "已回传" &&
                unbound.STA == "已出库"
                )
                    .OrderByDescending(unbound => unbound.CTI)
                    .ThenByDescending(unbound => unbound.ETI)
                    .ToList();
            }
        }
        [RelayCommand]
        public async Task OnHideUnboundAsync()
        {
            (DateTime startTime, DateTime endTime) dateTimeRang = await GetDateTimeRang("当天");
            UnboundList = SqliteHelper.Current.db.Unbounds.Where(unbound =>
            dateTimeRang.startTime.CompareTo((DateTime)(object)unbound.CTI) <= 0 && dateTimeRang.endTime.CompareTo((DateTime)(object)unbound.CTI) >= 0 &&
            unbound.UPSTATE != "已回传" &&
            unbound.STA != "已出库"
            )
                .OrderByDescending(unbound => unbound.CTI)
                .ThenByDescending(unbound => unbound.ETI)
                .ToList();
        }
        [RelayCommand]
        public async Task OnAddUnboundAsync()
        {
            IsDisabled = true;
            CurrentUnbound = new Unbound()
            {
                STA = "待出库",
                UPSTATE = "待回传",
                CTI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            KeyValuePairs = GetKeyValuePairs();
            scanService.Scan += ScanService_Scan;
            Shell.Current.Navigating += Current_Navigating;
            await Shell.Current.Navigation.PushAsync(new UnboundMoreActionsPage(), true);
            await Snackbar.Make("请按扫描键扫描对应的二维码", null, "知道了", TimeSpan.FromSeconds(5)).Show();
        }
        [RelayCommand(CanExecute = "IsDisabled")]
        public async Task OnSaveUnboundAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }
        [RelayCommand]
        public async Task OnEditUnboundAsync(Unbound unbound)
        {
            IsDisabled = true;
            CurrentUnbound = unbound;
            KeyValuePairs = GetKeyValuePairs();
            scanService.Scan += ScanService_Scan;
            Shell.Current.Navigating += Current_Navigating;
            await Shell.Current.Navigation.PushAsync(new UnboundMoreActionsPage(), true);
        }
        [RelayCommand]
        public async Task OnDeleteUnboundAsync(Unbound unbound)
        {
            IsDisabled = false;
            bool isDelete = await UnboundPage.DisplayAlert("警告", "你是否删除此条数据，该操作不可撤销？", "确认", "取消");
            if (isDelete)
            {
                SqliteHelper.Current.db.Remove(unbound);
                await SqliteHelper.Current.db.SaveChangesAsync();
                UnboundList = SqliteHelper.Current.db.Unbounds.Where(unbound =>
                unbound.UPSTATE != "已回传" &&
                unbound.STA != "已出库").ToList();
            }
        }
        [RelayCommand]
        public async Task OnGetInfoUnboundAsync(Unbound unbound)
        {
            IsDisabled = false;
            CurrentUnbound = unbound;
            KeyValuePairs = GetKeyValuePairs();
            await Shell.Current.Navigation.PushAsync(new UnboundMoreActionsPage(), true);
        }
        private async void Current_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.Source == ShellNavigationSource.PopToRoot && IsDisabled)
            {
                ShellNavigatingDeferral token = e.GetDeferral();
                if (string.IsNullOrEmpty(CurrentUnbound.CNO))
                {
                    token.Complete();
                    scanService.Scan -= ScanService_Scan;
                    Shell.Current.Navigating -= Current_Navigating;
                    return;
                }
                bool isSave = await UnboundPage.DisplayAlert("信息", "是否保存当前数据？", "确认", "取消");
                if (isSave)
                {
                    bool isStaChange = false;
                    foreach (KeyValuePair<string, object> keyValue in KeyValuePairs)
                    {
                        bool required = (bool)keyValue.Value.GetObjectPropertyValue("Required");
                        object unboundValue = CurrentUnbound.GetObjectPropertyValue(keyValue.Key);
                        if (required && unboundValue == null)
                        {
                            isStaChange = true;
                            break;
                        }
                    }
                    if (!isStaChange)
                    {
                        CurrentUnbound.STA = "已出库";
                        LoginViewModel loginViewModel = DependencyService.Get<LoginViewModel>();
                        PadUser user = loginViewModel.CurrentUser;
                        CurrentUnbound.EMP = user.CN_LOGIN;
                        CurrentUnbound.ETI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        Mdetail mdetail = SqliteHelper.Current.db.Mdetails.Where(mdetail => mdetail.RNO == CurrentUnbound.RNO).FirstOrDefault();
                        if (mdetail is not null)
                        {
                            Type type = CurrentUnbound.GetType();
                            PropertyInfo[] propertyInfos = type.GetProperties();
                            for (int i = 0; i < propertyInfos.Length; i++)
                            {
                                mdetail.TrySetObjectPropertyValue(propertyInfos[i].Name, CurrentUnbound.GetObjectPropertyValue(propertyInfos[i].Name));
                            }
                            decimal qua = decimal.Parse(mdetail.QUA);
                            decimal fqua = decimal.Parse(CurrentUnbound.FQUA);
                            decimal diff = qua - fqua;
                            mdetail.QUA = diff.ToString();
                            mdetail.CTI = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            SqliteHelper.Current.db.Update(mdetail);
                            SqliteHelper.Current.db.SaveChanges();
                        }
                    }
                    SqliteHelper.Current.db.AddOrUpdate(CurrentUnbound);
                    SqliteHelper.Current.db.SaveChanges();
                    UnboundList = SqliteHelper.Current.db.Unbounds.Where(unbound =>
                unbound.UPSTATE != "已回传" &&
                unbound.STA != "已出库").ToList();
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
                PropertyInfo[] propertyInfos = CurrentUnbound.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (CurrentUnbound.GetObjectPropertyValue(propertyInfo.Name) == null && keyValuePairs.TryGetValue(propertyInfo.Name, out string value))
                        CurrentUnbound.SetObjectPropertyValue(propertyInfo.Name, value);
                }
                if (CurrentUnbound.ICD is not null)
                {
                    Mbase mbase = SqliteHelper.Current.db.Mbases?.Where(mb => mb.ICD == CurrentUnbound.ICD).FirstOrDefault();
                    if (mbase is not null)
                    {
                        CurrentUnbound.INA = mbase.INA;
                        CurrentUnbound.UT = mbase.UT;
                        CurrentUnbound.ISJ = mbase.ISJ;
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
            UnboundPage.Dispatcher.Dispatch(async () =>
            {
                try
                {
                    CurrentUnbound.FQUA = await UnboundPage.DisplayPromptAsync("申请数据", "请输入申请数量", "确认", "取消", "请输入", -1, Keyboard.Numeric, CurrentUnbound.FQUA) ?? throw new NullReferenceException("必须填写申请数量");
                    if (string.IsNullOrWhiteSpace(CurrentUnbound.FQUA))
                        throw new NullReferenceException("必须填写申请数量");
                    KeyValuePairs = GetKeyValuePairs();
                }
                catch (Exception e)
                {
                    await MessageUtils.ShowPopuDangerAsync(e.Message);
                }
            });
        }
        private void EditLLRNAMEPZ()
        {
            UnboundPage.Dispatcher.Dispatch(async () =>
            {
                try
                {
                    IDrawingService drawingService = DependencyService.Get<IDrawingService>();
                    if (CurrentUnbound.CRNO is null)
                        throw new NullReferenceException("没有申请出库物资编号，不能签字");
                    bool isLoadImage = await drawingService.TryLoadImage(CurrentUnbound.CRNO);
                    if (isLoadImage)
                    {
                        drawingService.IsEnabled = false;
                        await UnboundPage.ShowPopupAsync(drawingService.DrawingPage);
                    }
                    else
                    {
                        await UnboundPage.ShowPopupAsync(drawingService.DrawingPage);
                        await drawingService.SaveImage(CurrentUnbound.CRNO);
                    }
                    //if (CurrentUnbound.STA == "已出库")
                    //{
                    //    drawingService.IsEnabled = false;
                    //    await drawingService.LoadImage(CurrentUnbound.CRNO);
                    //    await UnboundPage.ShowPopupAsync(drawingService.DrawingPage);
                    //}
                    //else
                    //{
                    //    await UnboundPage.ShowPopupAsync(drawingService.DrawingPage);
                    //    await drawingService.SaveImage(CurrentUnbound.CRNO);
                    //}
                    drawingService.Clear();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Sequence contains no elements"))
                        await MessageUtils.ShowPopuWarningAsync("不能为空，请签字");
                    else await MessageUtils.ShowPopuDangerAsync(e.Message);
                }
            });
        }

        public IDictionary<string, object> GetKeyValuePairs()
        {
            EditNumber = EditFQUA;
            EditDraw = EditLLRNAMEPZ;
            return
            new Dictionary<string, object>()
            {
                { "CNO", new { Title = "出库流水号：",Required = true,ActionMethod = NoMethod} },
                { "CRNO", new { Title = "申请出库物资编号：",Required = false,ActionMethod = NoMethod} },
                { "ICD", new { Title = "物料编码：",Required = false,ActionMethod = NoMethod} },
                { "QUA", new { Title = "申请数量：",Required = false,ActionMethod = NoMethod} },
                { "FQUA", new { Title = "实收数量：",Required = true,ActionMethod = EditNumber} },
                { "UT", new { Title = "计量单位：",Required = false,ActionMethod = NoMethod} },
                { "RNO", new { Title = "物资编号：",Required = false,ActionMethod = NoMethod} },
                { "KWCODE", new { Title = "库位：",Required = true,ActionMethod = NoMethod} },
                { "LLRNAMEPZ", new { Title = "领料人手绘凭证：",Required = false,ActionMethod = EditDraw} },
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
                    st.AddMonths(-1);
                    break;
                case "自定义":
                    (DateTime StartDate, DateTime EndDate)? dateRang = await UnboundPage.ShowPopupAsync(new DateRangPage()) as (DateTime StartDate, DateTime EndDate)?;
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
