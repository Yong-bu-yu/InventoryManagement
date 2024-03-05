using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Historical;

public partial class HistoricalPage : ContentPage
{
	public HistoricalPage()
	{
		InitializeComponent();
        HistoricalViewModel historicalViewModel = new HistoricalViewModel();
        historicalViewModel.HistoricalPage = this;
        DependencyService.RegisterSingleton(historicalViewModel);
        BindingContext = historicalViewModel;
    }

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        KeyValuePair<string, object> pairs = (KeyValuePair<string, object>)e.Item;
        Action method = pairs.Value.GetObjectPropertyValue("ActionMethod") as Action;
        method?.Invoke();
    }
}