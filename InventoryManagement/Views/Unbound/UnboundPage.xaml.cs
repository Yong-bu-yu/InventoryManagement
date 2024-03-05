using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Unbound;

public partial class UnboundPage : ContentPage
{
    public UnboundPage()
    {
        InitializeComponent();
        UnboundViewModel unboundViewModel = new UnboundViewModel();
        unboundViewModel.UnboundPage = this;
        DependencyService.RegisterSingleton(unboundViewModel);
        BindingContext = unboundViewModel;
        Loaded += (sender, e) => Focus();
    }
}