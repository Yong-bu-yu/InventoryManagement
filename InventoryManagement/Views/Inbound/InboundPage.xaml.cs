using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Inbound;

public partial class InboundPage : ContentPage
{
	public InboundPage()
	{
		InitializeComponent();
		InboundViewModel inboundViewModel = new InboundViewModel();
		inboundViewModel.InboundPage = this;
		DependencyService.RegisterSingleton(inboundViewModel);
		BindingContext = inboundViewModel;
        Loaded += (sender, e) => Focus();
    }
}