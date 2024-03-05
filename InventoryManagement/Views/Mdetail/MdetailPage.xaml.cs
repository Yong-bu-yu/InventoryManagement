using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.ViewModel;

namespace InventoryManagement.Views.Mdetail;

public partial class MdetailPage : ContentPage
{
    public MdetailPage()
    {
        InitializeComponent();
        MdetailViewModel mdetailViewModel = new MdetailViewModel();
        mdetailViewModel.MdetailPage = this;
        DependencyService.RegisterSingleton(mdetailViewModel);
        BindingContext = mdetailViewModel;
        //if (SqliteHelper.Current.db.Mdetails.FirstOrDefault() is null)
        //{
        //    Model.Mdetail mdetail = new Model.Mdetail();
        //    mdetail.UUID = Guid.NewGuid().ToString();
        //    mdetail.RNO = "1";
        //    mdetail.ICD = "1";
        //    mdetail.INA = "≤‚ ‘";
        //    SqliteHelper.Current.db.Mdetails.Add(mdetail);
        //    SqliteHelper.Current.db.SaveChanges();
        //}
    }
}