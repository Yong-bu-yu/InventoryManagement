using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Inventory;

public partial class InventoryMoreActionsPage : ContentPage
{
	public InventoryMoreActionsPage()
	{
		InitializeComponent();
        BindingContext = DependencyService.Get<InventoryViewModel>();
    }

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        KeyValuePair<string, object> pairs = (KeyValuePair<string, object>)e.Item;
        Action method = pairs.Value.GetObjectPropertyValue("ActionMethod") as Action;
        method?.Invoke();
    }
}