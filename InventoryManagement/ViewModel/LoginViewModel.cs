using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Service.Login;
using InventoryManagement.Service.NetWork;
using InventoryManagement.Utils;
using InventoryManagement.Views.Inbound;
using InventoryManagement.Views.Login;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.ViewModel;

public partial class LoginViewModel : ObservableValidator
{
    private LoginService LoginService { get; } = DependencyService.Get<LoginService>();
    public PadUser CurrentUser { get; set; }
    public LoginPage LoginPage { get; set; }
    private LoginSettingPage LoginSettingPage { get; set; }
    public string ipAddress;

    [Required(ErrorMessage = "必须输入服务器IP地址")]
    [RegularExpression("(ht|f)tp(s?)\\:\\/\\/[0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*(:(0-9)*)*(\\/?)([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\&%\\+\\$#_=]*)?", ErrorMessage = "不是正确的http地址")]
    public string IpAddress { get => ipAddress; set => SetProperty(ref ipAddress, value, true, nameof(IpAddress)); }
    private IList<string> pickerItemsSource;
    public IList<string> PickerItemsSource { get { return pickerItemsSource; } set => SetProperty(ref pickerItemsSource, value, true, nameof(PickerItemsSource)); }
    private string selectedUserGroup;
    public string SelectedUserGroup { get => selectedUserGroup; set => SetProperty(ref selectedUserGroup, value, true, nameof(SelectedUserGroup)); }

    private string userName;
    [Required(ErrorMessage = "必须填写用户名")]
    public string UserName { get => userName; set => SetProperty(ref userName, value, true, nameof(UserName)); }
    private string passWord;
    [Required(ErrorMessage = "必须填写密码")]
    public string PassWord { get => passWord; set => SetProperty(ref passWord, value, true, nameof(PassWord)); }
    public LoginViewModel() { }
    public string GetIpAddress()
    {
        SystemManagement systemManagement = SqliteHelper.Current.db.SystemManagements.FirstOrDefault();
        IpAddress = systemManagement?.IpAddress ?? throw new Exception("没有设置IP地址");
        return IpAddress;
    }
    [RelayCommand]
    public void OnOpenLoginSettingDialog()
    {
        SystemManagement systemManagement = SqliteHelper.Current.db.SystemManagements.FirstOrDefault();
        IpAddress = systemManagement?.IpAddress;
        LoginSettingPage = new LoginSettingPage();
        LoginPage?.ShowPopupAsync(LoginSettingPage);
        Task.Run(GetUserGroups);
    }

    [RelayCommand]
    public void OnCloseLoginSettingDialog()
    {
        LoginSettingPage.Close();
    }

    [RelayCommand]
    public async Task OnSaveSettingAsync()
    {
        ValidateProperty(IpAddress, nameof(IpAddress));
        if (HasErrors)
        {
            IList<ValidationResult> validationResults = GetErrors().ToList();
            for (int i = 0; i < validationResults.Count; i++)
                await MessageUtils.ShowPopuDangerAsync(validationResults[i].ErrorMessage);
            ClearErrors();
            return;
        }
        SystemManagement systemManagement = SqliteHelper.Current.db.SystemManagements.FirstOrDefault() ?? new SystemManagement();
        systemManagement.IpAddress = IpAddress;
        systemManagement.UUID = systemManagement?.UUID ?? Guid.NewGuid().ToString();
        SqliteHelper.Current.db.AddOrUpdate(systemManagement);
        SqliteHelper.Current.db.SaveChanges();
        await MessageUtils.ShowPopuSuccessAsync("保存成功");
        await GetUserGroups();
    }

    [RelayCommand]
    public async Task OnSystemUserImport()
    {
        if (SelectedUserGroup is null)
        {
            await MessageUtils.ShowPopuWarningAsync("请选择一个用户组");
            return;
        }
        try
        {
            LoadingUtils.LoadingStart("正在获取用户");
            IList<PadUser> padUserList = await NetWorkService.For<IPadUserService>().GetUserList(new Dictionary<string, string> { { "groupName", SelectedUserGroup } });
            IList<PadUser> padUsers = SqliteHelper.Current.db.PadUsers.ToList();
            IList<PadUser> exceptPadUsers = padUsers.Count >= padUserList.Count ? padUsers.Where(pad => !padUserList.Any(x => x.sys_key_uuid == pad.sys_key_uuid)).ToList() : padUserList.Where(pad => !padUsers.Any(x => x.sys_key_uuid == pad.sys_key_uuid)).ToList();//差集添加
            IList<PadUser> intersectPadUsers = padUsers.Where(pad => padUserList.Any(x => x.sys_key_uuid == pad.sys_key_uuid)).ToList();//交集更新
            await SqliteHelper.Current.db.AddRangeAsync(exceptPadUsers);
            await Task.Run(() => SqliteHelper.Current.db.UpdateRange(intersectPadUsers));
            await SqliteHelper.Current.db.SaveChangesAsync();
            LoadingUtils.LoadingEnd();
            await MessageUtils.ShowPopuSuccessAsync("用户导入成功");
        }
        catch (Exception e)
        {
            LoadingUtils.LoadingEnd();
            await MessageUtils.ShowPopuDangerAsync(e.Message);
        }
    }

    private async Task GetUserGroups()
    {
        try
        {
            if (IpAddress is not null)
            {
                LoginSettingPage.Dispatcher.Dispatch(() => Toast.Make("正在获取用户组...", CommunityToolkit.Maui.Core.ToastDuration.Long).Show());
                IList<string> userGroup = await NetWorkService.For<IPadUserService>().GetUserGroups();
                PickerItemsSource = userGroup;
                LoginSettingPage.Dispatcher.Dispatch(() => Toast.Make("获取用户组成功", CommunityToolkit.Maui.Core.ToastDuration.Long).Show());
            }
        }
        catch (Exception e)
        {
            if (e.Message.Contains("connect"))
                LoginSettingPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger("不能连接服务器，请确认IP是否正确"));
            else
                LoginSettingPage.Dispatcher.Dispatch(() => MessageUtils.ShowPopuDanger(e.Message));
        }
    }

    [RelayCommand]
    public async Task OnLoginAsync()
    {
        ValidateProperty(UserName, nameof(UserName));
        ValidateProperty(PassWord, nameof(PassWord));
        if (HasErrors)
        {
            IList<ValidationResult> validationResults = GetErrors().ToList();
            for (int i = 0; i < validationResults.Count; i++)
                await MessageUtils.ShowPopuDangerAsync(validationResults[i].ErrorMessage);
            ClearErrors();
            return;
        }
        try
        {
            CurrentUser = LoginService.Login(UserName, PassWord);
            await Shell.Current.GoToAsync($"//{nameof(InboundPage)}");
        }
        catch (Exception e)
        {
            await MessageUtils.ShowPopuDangerAsync(e.Message);
            return;
        }
    }
}