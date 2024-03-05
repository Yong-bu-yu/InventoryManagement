using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Windows.Input;

namespace InventoryManagement.Views.Template;

public partial class DateRangPage : Popup
{
    public DateRangPage()
    {
        InitializeComponent();
        EndDatePicker.MaximumDate = DateTime.Today;
        StartDatePicker.MaximumDate = EndDatePicker.Date;
        EndDatePicker.PropertyChanging += EndDatePicker_PropertyChanging; ;
    }

    private void EndDatePicker_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {
        Dispatcher.Dispatch(() =>
        {
            if (StartDatePicker.Date > EndDatePicker.Date)
                StartDatePicker.Date = EndDatePicker.Date;
            StartDatePicker.MaximumDate = EndDatePicker.Date;
        });
    }

    async void OnYesButtonClicked(object sender, EventArgs e) => await CloseAsync((StartDate: StartDatePicker.Date, EndDate: EndDatePicker.Date));

    async void OnNoButtonClicked(object sender, EventArgs e) => await CloseAsync();
}