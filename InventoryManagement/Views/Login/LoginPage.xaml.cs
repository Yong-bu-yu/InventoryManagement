using CommunityToolkit.Maui.Views;
using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Service.AutoApp;
using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Login;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        LoginViewModel loginViewModel = new LoginViewModel();
        loginViewModel.LoginPage = this;
        DependencyService.RegisterSingleton(loginViewModel);
        if (SqliteHelper.Current.db.PadUsers.FirstOrDefault() is null)
        {
            PadUser pad = new PadUser()
            {
                sys_key_uuid = Guid.NewGuid().ToString(),
                CN_LOGIN = "xm",
                CN_Download_Password = "p1nq6k176u7DLnZaRaCDxA=="
            };
            SqliteHelper.Current.db.Add(pad);
            SqliteHelper.Current.db.SaveChanges();
        }
        BindingContext = loginViewModel;
        IAutoAppService autoAppService = new AutoAppService();
        //autoAppService.IsAutoApp = true;
        DependencyService.RegisterSingleton<IAutoAppService>(autoAppService);
    }
}