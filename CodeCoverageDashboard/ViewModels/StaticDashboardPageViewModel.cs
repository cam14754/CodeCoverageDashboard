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

namespace CodeCoverageDashboard.ViewModels;

public partial class StaticDashboardPageViewModel(IDataHandlerService dataHandlerService) : BaseViewModel
{
    readonly IDataHandlerService dataHandlerService = dataHandlerService;
    public ObservableCollection<StaticDashboardData>? Data => dataHandlerService.Latest;
    public StaticDashboardData? LatestData => Data?.FirstOrDefault();
    public StaticDashboardData? WeekOldData { get; set; }

    [RelayCommand]
    public async Task GetRepoData()
    {
        if (IsBusy)
        {
            return;
        }
        IsBusy = true;
        try
        {
            await Execute();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading latest repos: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task Execute()
    {
        Debug.WriteLine("Starting calls...");

        // Start DB Call
        await Task.Run(async () =>
        {
            await dataHandlerService.LoadLatestStaticDashboardData();
        });
        

        //Find week old data
        var target = DateTime.Now.AddDays(-7);
        var start = target.AddHours(-12);
        var end = target.AddHours(12);
        WeekOldData = Data?.Where(d => d.DateRetrieved >= start && d.DateRetrieved <= end).FirstOrDefault();

        WeekOldData ??= new StaticDashboardData();

        OnPropertyChanged(nameof(Data));
        OnPropertyChanged(nameof(LatestData));
        OnPropertyChanged(nameof(WeekOldData));

        Debug.WriteLine("Execute finished (UI applied).");
    }

    

    [RelayCommand]
    public async Task GoToMainPageAsync()
    {
        if (IsBusy)
        {
            return;
        }
        IsBusy = true;
        try
        {
            await Shell.Current.GoToAsync($"{nameof(DrillDownDashboardPage)}", true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error navigating to DrillDownDashboardPage: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
