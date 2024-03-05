using InventoryManagement.Utils;
using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Unbound;

public partial class UnboundMoreActionsPage : ContentPage
{
    public UnboundMoreActionsPage()
    {
        InitializeComponent();
        BindingContext = DependencyService.Get<UnboundViewModel>();
    }

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        KeyValuePair<string, object> pairs = (KeyValuePair<string, object>)e.Item;
        Action method = pairs.Value.GetObjectPropertyValue("ActionMethod") as Action;
        method?.Invoke();
    }
}