using CommunityToolkit.Maui.Views;
using InventoryManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Utils
{
    internal static class LoadingUtils
    {
        private static LoadingPage LoadingPage { get; set; }
        private static CancellationTokenSource Source { get; set; }

        public static void LoadingStart(string loadingText)
        {
            LoadingPage = new LoadingPage();
            Source = new CancellationTokenSource();
            LoadingPage.Text = loadingText;
            Task.Run(() =>
            {
                string oldText = loadingText;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(oldText);
                int i = 0;
                while (true)
                {
                    if (i != 6)
                    {
                        LoadingPage.Dispatcher.Dispatch(() =>
                        {
                            stringBuilder.Append('.');
                            LoadingPage.Text = stringBuilder.ToString();
                        });
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        LoadingPage.Dispatcher.Dispatch(() =>
                        {
                            stringBuilder.Clear();
                            stringBuilder.Append(oldText);
                            LoadingPage.Text = stringBuilder.ToString();
                            i = 0;
                        });
                    }
                    i++;
                }
            }, Source.Token);
            Application.Current.MainPage.ShowPopup(LoadingPage);
        }

        public static void LoadingEnd()
        {
            Source.Cancel();
            Source.Dispose();
            LoadingPage.Close();
        }
    }
}
