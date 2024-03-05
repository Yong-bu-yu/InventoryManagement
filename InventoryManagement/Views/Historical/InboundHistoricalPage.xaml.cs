using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Historical;

public partial class InboundHistoricalPage : ContentPage
{
	public InboundHistoricalPage()
	{
		InitializeComponent();
        BindingContext = DependencyService.Get<HistoricalViewModel>();
    }
}