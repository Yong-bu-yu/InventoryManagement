using InventoryManagement.Utils;
using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Inbound;

public partial class InboundMoreActionsPage : ContentPage
{
    public InboundMoreActionsPage()
    {
        InitializeComponent();
        BindingContext = DependencyService.Get<InboundViewModel>();
    }

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        KeyValuePair<string, object> pairs = (KeyValuePair<string, object>)e.Item;
        Action method = pairs.Value.GetObjectPropertyValue("ActionMethod") as Action;
        method?.Invoke();
    }
}