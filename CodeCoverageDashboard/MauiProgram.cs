// COPYRIGHT © 2025 ESRI
//
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States.
//
// For additional information, contact:
// Environmental Systems Research Institute, Inc.
// Attn: Contracts Dept
// 380 New York Street
// Redlands, California, USA 92373
//
// email: contracts@esri.com

using Telerik.Maui.Controls.Compatibility;

namespace CodeCoverageDashboard;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseTelerik()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<DrillDownDashboardPageViewModel>();
        builder.Services.AddSingleton<RepoPageViewModel>();
        builder.Services.AddSingleton<ClassPageViewModel>();
        builder.Services.AddSingleton<StaticDashboardPageViewModel>();

        builder.Services.AddSingleton<DrillDownDashboardPage>();
        builder.Services.AddTransient<RepoPage>();
        builder.Services.AddTransient<ClassPage>();
        builder.Services.AddSingleton<StaticDashboardPage>();

        builder.Services.AddSingleton<IDataHandlerService, DataHandlerService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

        return builder.Build();
    }
}
