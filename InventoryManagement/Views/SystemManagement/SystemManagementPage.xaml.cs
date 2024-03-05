using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.SystemManagement;

public partial class SystemManagementPage : ContentPage
{
	public SystemManagementPage()
	{
		InitializeComponent();
        SystemManagementViewModel systemManagementViewModel = new SystemManagementViewModel();
        systemManagementViewModel.SystemManagementPage = this;
        DependencyService.RegisterSingleton(systemManagementViewModel);
        BindingContext = systemManagementViewModel;
    }

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        KeyValuePair<string, object> pairs = (KeyValuePair<string, object>)e.Item;
        Action method = pairs.Value.GetObjectPropertyValue("ActionMethod") as Action;
        method?.Invoke();
    }
}