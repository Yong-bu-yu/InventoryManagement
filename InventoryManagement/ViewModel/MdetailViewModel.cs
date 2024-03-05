using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Views.Mdetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.ViewModel
{
    internal partial class MdetailViewModel : ObservableValidator
    {
        private Action NoMethod;
        public Mdetail CurrentMdetail { get; set; }
        public MdetailPage MdetailPage { get; set; }
        private List<Mdetail> mdetailList;
        public List<Mdetail> MdetailList { get => mdetailList; set => SetProperty(ref mdetailList, value, true); }

        public IDictionary<string, object> keyValuePairs;
        public IDictionary<string, object> KeyValuePairs { get => keyValuePairs; set => SetProperty(ref keyValuePairs, value, true); }

        private string rno = "";
        public string RNO { get => rno; set => SetProperty(ref rno, value, true); }
        private string icd = "";
        public string ICD { get => icd; set => SetProperty(ref icd, value, true); }
        private string ina = "";
        public string INA { get => ina; set => SetProperty(ref ina, value, true); }
        public MdetailViewModel()
        {
            KeyValuePairs = GetKeyValuePairs();
            MdetailList = SqliteHelper.Current.db.Mdetails.ToList();
        }
        [RelayCommand]
        public async Task OnGetInfoMdetailAsync(Mdetail mdetail)
        {
            CurrentMdetail = mdetail;
            KeyValuePairs = GetKeyValuePairs();
            await Shell.Current.Navigation.PushAsync(new MdetailInfoPage(), true);
        }
        [RelayCommand]
        public void OnGetMdetailList()
        {
            MdetailList = SqliteHelper.Current.db.Mdetails.Where(mdetail => mdetail.RNO.Contains(RNO) && mdetail.ICD.Contains(ICD) && mdetail.INA.Contains(INA)).ToList();
            KeyValuePairs = GetKeyValuePairs();
        }
        public IDictionary<string, object> GetKeyValuePairs()
        {
            return
            new Dictionary<string, object>()
            {
                { "RNO", new { Title = "物资编号：",Required = false,ActionMethod = NoMethod} },
                { "ICD", new { Title = "物料编码：",Required = false,ActionMethod = NoMethod} },
                { "INA", new { Title = "物料名称：",Required = false,ActionMethod = NoMethod} },
                { "QUA", new { Title = "数量：",Required = false,ActionMethod = NoMethod} },
                { "UT", new { Title = "计量单位：",Required = false,ActionMethod = NoMethod} },
                { "ISJ", new { Title = "是否集团物资码：",Required = false,ActionMethod = NoMethod} },
                { "PCD", new { Title = "产品编号：",Required = false,ActionMethod = NoMethod} },
                { "KWCODE", new { Title = "库位编号：",Required = false,ActionMethod = NoMethod} },
                { "KWNAME", new { Title = "库位名称：",Required = false,ActionMethod = NoMethod} },
                { "KQNAME", new { Title = "库区名称：",Required = false,ActionMethod = NoMethod} },
                { "CKNAME", new { Title = "仓库名称：",Required = false,ActionMethod = NoMethod} },
                { "PDTI", new { Title = "最后一次盘点时间：",Required = false,ActionMethod = NoMethod} },
                { "RTI", new { Title = "入库时间：",Required = false,ActionMethod = NoMethod} },
                { "CTI", new { Title = "最后一次出库时间：",Required = false,ActionMethod = NoMethod} },
            };
        }
    }
}
