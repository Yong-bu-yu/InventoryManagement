using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Historical;

public partial class UnboundHistoricalPage : ContentPage
{
	public UnboundHistoricalPage()
	{
		InitializeComponent();
        BindingContext = DependencyService.Get<HistoricalViewModel>();
    }
}