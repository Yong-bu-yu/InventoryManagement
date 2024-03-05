using CommunityToolkit.Maui.Views;
using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Login;
public partial class LoginSettingPage : Popup
{
	public LoginSettingPage()
	{
		InitializeComponent();
        BindingContext = DependencyService.Get<LoginViewModel>();
    }
}