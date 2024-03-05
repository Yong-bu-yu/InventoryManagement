using CommunityToolkit.Maui.Views;
using InventoryManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InventoryManagement.Views.MessagePage;

namespace InventoryManagement.Utils
{
    internal static class MessageUtils
    {

        public static void ShowPopu(MessageType messageType, string message, string title = null)
        {
            Application.Current.MainPage.ShowPopup(new MessagePage().ShowPopu(messageType, message, title));
        }
        public static async void ShowPopuAsync(MessageType messageType, string message, string title = null)
        {
            await Application.Current.MainPage.ShowPopupAsync(await new MessagePage().ShowPopuAsync(messageType, message, title));
        }
        public static void ShowPopuSuccess(string message, string title = null)
        {
            Application.Current.MainPage.ShowPopup(new MessagePage().ShowPopuSuccess(message, title));
        }
        public static void ShowPopuWarning(string message, string title = null)
        {
            Application.Current.MainPage.ShowPopup(new MessagePage().ShowPopuWarning(message, title));
        }
        public static void ShowPopuDanger(string message, string title = null)
        {
            Application.Current.MainPage.ShowPopup(new MessagePage().ShowPopuDanger(message, title));
        }
        public static async Task ShowPopuSuccessAsync(string message, string title = null)
        {
            await Application.Current.MainPage.ShowPopupAsync(await new MessagePage().ShowPopuSuccessAsync(message, title));
        }
        public static async Task ShowPopuWarningAsync(string message, string title = null)
        {
            await Application.Current.MainPage.ShowPopupAsync(await new MessagePage().ShowPopuWarningAsync(message, title));
        }
        public static async Task ShowPopuDangerAsync(string message, string title = null)
        {
            await Application.Current.MainPage.ShowPopupAsync(await new MessagePage().ShowPopuDangerAsync(message, title));
        }
    }
}
