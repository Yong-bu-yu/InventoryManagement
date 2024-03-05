using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Mdetail;

public partial class MdetailInfoPage : ContentPage
{
	public MdetailInfoPage()
	{
		InitializeComponent();
		BindingContext = DependencyService.Get<MdetailViewModel>();
	}
}