using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Service.AutoApp;
using InventoryManagement.Service.Drawing;
using InventoryManagement.Service.Login;
using InventoryManagement.Service.Scanner;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace InventoryManagement
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureLifecycleEvents(events =>
                {
#if ANDROID
                    events.AddAndroid(android => android.OnCreate((activity, bundle) =>
                    {
                        DependencyService.RegisterSingleton<AutoUpdate.Services.IAppInfo>(new AutoUpdate.Droid.AppInfo());
                        DependencyService.RegisterSingleton<AutoUpdate.Services.IFileOpener>(new AutoUpdate.Droid.FileOpener());
                        DependencyService.RegisterSingleton<AutoUpdate.Services.IStoreOpener>(new AutoUpdate.Droid.PlayStoreOpener());
                        AutoUpdate.Droid.AutoUpdate.Init(activity, $"{activity.PackageName}.fileProvider");
                    }));
#endif
                });
            ;
#if DEBUG
            builder.Logging.AddDebug();
#endif
            DependencyService.RegisterSingleton<IEFCoreDbSQLite>(new SQLiteAndroid());
            DependencyService.RegisterSingleton<LoginService>(new LoginService());
            DependencyService.RegisterSingleton<ScanService>(new ScanService());
            DependencyService.RegisterSingleton<IDrawingService>(new DrawingService());

            SqliteHelper.Current.CreateOrUpdateAllTables();
            return builder.Build();
        }
    }
}