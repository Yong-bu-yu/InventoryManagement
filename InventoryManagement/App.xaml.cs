using InventoryManagement.Views;

namespace InventoryManagement
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}