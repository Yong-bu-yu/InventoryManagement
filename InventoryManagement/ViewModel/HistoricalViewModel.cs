using CommunityToolkit.Mvvm.ComponentModel;
using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Views.Historical;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace InventoryManagement.ViewModel
{
    internal partial class HistoricalViewModel : ObservableValidator
    {
        private Action ToInboundHistorical;
        private Action ToUnboundHistorical;
        private Action ToInventoryHistorical;
        public HistoricalPage HistoricalPage { get; set; }
        public IDictionary<string, object> keyValuePairs;
        public IDictionary<string, object> KeyValuePairs { get => keyValuePairs; set => SetProperty(ref keyValuePairs, value, true); }

        private List<Inbound> inboundList;
        public List<Inbound> InboundList { get => inboundList; set => SetProperty(ref inboundList, value, true); }
        private List<Unbound> unboundList;
        public List<Unbound> UnboundList { get => unboundList; set => SetProperty(ref unboundList, value, true); }
        private List<Inventory> inventoryList;
        public List<Inventory> InventoryList { get => inventoryList; set => SetProperty(ref inventoryList, value, true); }
        private string dateTimeSelect = "全部";
        private Type currentType;
        public Type CurrentType
        {
            get => currentType; set
            {
                if (value == inboundType)
                    GetInboundList(DateTimeSelect);
                else if (value == unboundType)
                    GetUnboundList(DateTimeSelect);
                else if (value == inventoryType)
                    GetInventoryList(DateTimeSelect);
                SetProperty(ref currentType, value, true);
            }
        }
        private Type inboundType = typeof(Inbound);
        private Type unboundType = typeof(Unbound);
        private Type inventoryType = typeof(Inventory);
        public string DateTimeSelect
        {
            get => dateTimeSelect;
            set
            {
                if (CurrentType == inboundType)
                    GetInboundList(value);
                else if (CurrentType == unboundType)
                    GetUnboundList(value);
                else if (CurrentType == inventoryType)
                    GetInventoryList(value);
                SetProperty(ref dateTimeSelect, value, true);
            }
        }

        public HistoricalViewModel()
        {
            KeyValuePairs = GetKeyValuePairs();
        }

        private void GoToInboundHistorical()
        {
            CurrentType = inboundType;
            Shell.Current.Navigation.PushAsync(new InboundHistoricalPage());
        }
        private void GoToUnboundHistorical()
        {
            CurrentType = unboundType;
            Shell.Current.Navigation.PushAsync(new UnboundHistoricalPage());
        }
        private void GoToInventoryHistorical()
        {
            CurrentType = inventoryType;
            Shell.Current.Navigation.PushAsync(new InventoryHistoricalPage());
        }
        private void GetInboundList(string datetime)
        {
            (DateTime startTime, DateTime endTime) dateTimeRang = GetDateTimeRang(datetime);
            InboundList = SqliteHelper.Current.db.Inbounds.Where(inbound => dateTimeRang.startTime.CompareTo((DateTime)(object)inbound.CTI) <= 0 && dateTimeRang.endTime.CompareTo((DateTime)(object)inbound.CTI) >= 0).ToList();
        }
        private void GetUnboundList(string datetime)
        {
            (DateTime startTime, DateTime endTime) dateTimeRang = GetDateTimeRang(datetime);
            UnboundList = SqliteHelper.Current.db.Unbounds.Where(unbound => dateTimeRang.startTime.CompareTo((DateTime)(object)unbound.CTI) <= 0 && dateTimeRang.endTime.CompareTo((DateTime)(object)unbound.CTI) >= 0).ToList();
        }
        private void GetInventoryList(string datetime)
        {
            (DateTime startTime, DateTime endTime) dateTimeRang = GetDateTimeRang(datetime);
            InventoryList = SqliteHelper.Current.db.Inventories.Where(inventory => dateTimeRang.startTime.CompareTo((DateTime)(object)inventory.CTI) <= 0 && dateTimeRang.endTime.CompareTo((DateTime)(object)inventory.CTI) >= 0).ToList();
        }
        private (DateTime startTime, DateTime endTime) GetDateTimeRang(string datetime)
        {
            DateTime st = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime et = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            switch (datetime)
            {
                case "一周前":
                    st = st.AddDays(-7);
                    break;
                case "一个月前":
                    st = st.AddMonths(-1);
                    break;
                case "三个月前":
                    st = st.AddMonths(-3);
                    break;
                case "全部":
                    st = DateTime.MinValue;
                    et = DateTime.MaxValue;
                    break;
                default:
                    st = DateTime.MinValue;
                    et = DateTime.MaxValue;
                    break;
            }
            return (st, et);
        }
        public IDictionary<string, object> GetKeyValuePairs()
        {
            ToInboundHistorical = GoToInboundHistorical;
            ToUnboundHistorical = GoToUnboundHistorical;
            ToInventoryHistorical = GoToInventoryHistorical;
            return
            new Dictionary<string, object>()
            {
                { Guid.NewGuid().ToString(), new { Title = "入库历史记录",Required = true,ActionMethod = ToInboundHistorical} },
                { Guid.NewGuid().ToString(), new { Title = "出库历史记录",Required = false,ActionMethod = ToUnboundHistorical} },
                { Guid.NewGuid().ToString(), new { Title = "盘点历史记录",Required = false,ActionMethod = ToInventoryHistorical} },
            };
        }
    }
}
