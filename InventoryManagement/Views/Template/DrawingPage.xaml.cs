using CommunityToolkit.Maui.Views;

namespace InventoryManagement.Views.Template;

public partial class DrawingPage : Popup
{
    public DrawingPage()
    {
        InitializeComponent();
    }
    public DrawingView DrawingView { get => DrawingViewControl; }
    //public static readonly BindableProperty IsEnabledProperty = BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(DrawingPage), true);
    //public bool IsEnabled
    //{
    //    get => DrawingViewControl.IsEnabled;
    //    set
    //    {
    //        SetValue(IsEnabledProperty, value);
    //        DrawingViewControl.IsEnabled = value;
    //    }
    //}
    public bool IsEnabled { get => DrawingViewControl.IsEnabled; set => DrawingViewControl.IsEnabled = value; }

    async void OnYesButtonClicked(object sender, EventArgs e) => await CloseAsync();

    async void OnNoButtonClicked(object sender, EventArgs e) => await CloseAsync();
}