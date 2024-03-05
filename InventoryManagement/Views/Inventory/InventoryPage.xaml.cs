using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Inventory;

public partial class InventoryPage : ContentPage
{
	public InventoryPage()
	{
		InitializeComponent();
        InventoryViewModel inventoryViewModel = new InventoryViewModel();
        inventoryViewModel.InventoryPage = this;
        DependencyService.RegisterSingleton(inventoryViewModel);
        BindingContext = inventoryViewModel;
        Loaded += (sender, e) => Focus();
    }
}