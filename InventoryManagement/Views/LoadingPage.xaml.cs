using CommunityToolkit.Maui.Views;

namespace InventoryManagement.Views;

public partial class LoadingPage : Popup
{
	public string Text { get => LoadingText.Text; set => LoadingText.Text = value; }

    public LoadingPage()
	{
		InitializeComponent();
	}
}