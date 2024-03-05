using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Historical;

public partial class InventoryHistoricalPage : ContentPage
{
	public InventoryHistoricalPage()
	{
		InitializeComponent();
        BindingContext = DependencyService.Get<HistoricalViewModel>();
    }
}