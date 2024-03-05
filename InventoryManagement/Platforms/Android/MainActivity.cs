using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using InventoryManagement.Service.AutoApp;
using InventoryManagement.Service.Scanner;
using System.Runtime.Versioning;

namespace InventoryManagement
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        [ObsoletedOSPlatform("Android 5.0")]
        public override bool OnKeyMultiple([GeneratedEnum] Keycode keyCode, int repeatCount, KeyEvent e)
        {
            ScanService scanService = DependencyService.Get<ScanService>();
            scanService.OnScan(new ScanEventArgs(e.Characters));
            return base.OnKeyMultiple(keyCode, repeatCount, e);
        }
    }
}