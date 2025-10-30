// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionCore()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton<RepoPageViewModel>();
		builder.Services.AddSingleton<ClassPageViewModel>();
		builder.Services.AddSingleton<MethodPageViewModel>();
		builder.Services.AddSingleton<StaticDashboardPageViewModel>();

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<RepoPage>();
		builder.Services.AddTransient<ClassPage>();
		builder.Services.AddTransient<MethodPage>();
		builder.Services.AddSingleton<StaticDashboardPage>();

		builder.Services.AddSingleton<IDataHandlerService, DataHandlerService>();
		builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

		return builder.Build();
	}
}