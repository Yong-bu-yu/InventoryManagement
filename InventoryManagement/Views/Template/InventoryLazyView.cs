using CommunityToolkit.Maui.Views;
using InventoryManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Views.Template
{
    internal class InventoryLazyView<T> : LazyView<T> where T : View, new()
    {
        public override async ValueTask LoadViewAsync()
        {
            Content = new ActivityIndicator { IsRunning = true };
            T t = new T { BindingContext = BindingContext };
            await Task.Run(() =>
            {
                while (!IsLoaded) ;
            });
            Content = t;
            SetHasLazyViewLoaded(true);
        }
    }
}
