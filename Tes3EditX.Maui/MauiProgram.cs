using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using Tes3EditX.Maui.Services;
using Tes3EditX.Backend.Services;
using Tes3EditX.Backend.ViewModels;

namespace Tes3EditX.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterAppServices()
                .RegisterViewModels()
                .RegisterViews();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<ISettingsService, SettingsService>();
            mauiAppBuilder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            mauiAppBuilder.Services.AddSingleton<ICompareService, CompareService>();

            mauiAppBuilder.Services.AddSingleton<IFileApiService, FileApiService>();
            mauiAppBuilder.Services.AddSingleton<IFolderPicker>(FolderPicker.Default);

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<ConflictsViewModel>();
            mauiAppBuilder.Services.AddTransient<AboutViewModel>();
            mauiAppBuilder.Services.AddTransient<PluginSelectViewModel>();
           

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<Views.MainPage>();
            mauiAppBuilder.Services.AddSingleton<Views.AboutPage>();
            mauiAppBuilder.Services.AddSingleton<Views.PluginSelectPage>();


            return mauiAppBuilder;
        }
    }
}