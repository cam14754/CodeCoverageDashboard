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

using CommunityToolkit.Maui.Extensions;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace CodeCoverageDashboard.ViewModels;

public partial class DrillDownDashboardPageViewModel(IDataHandlerService dataHandlerService) : BaseViewModel
{
    [ObservableProperty] public partial StaticDashboardData? LatestData { get; set; }
    [ObservableProperty] public partial StaticDashboardData? VisableData { get; set; }

    readonly IDataHandlerService datahandlerService = dataHandlerService;

    [RelayCommand]
    public async Task RunAsync()
    {
        if (IsBusy)
        {
            return;
        }
        try
        {
            IsBusy = true;
            Debug.WriteLine("Loading repos... \n");

            // Get latest data from service
            LatestData = await Task.Run(datahandlerService.GetLatestData);

            //Perform calculations off UI thread
            var Stats = await Task.Run(() => { return CalculatedStatsOnVisableRepos([.. LatestData.ListRepos]);});

            // Assign processed data to properties
            (LatestData.TotalLinesCount, LatestData.TotalLinesCoveredCount, LatestData.AverageLineCoveragePercent, LatestData.AverageBranchCoveragePercent) = Stats;
            VisableData = LatestData;

            OnPropertyChanged(nameof(VisableData));

            Debug.WriteLine("Repos loaded successfully.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading repos: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public static (double, double, double, double) CalculatedStatsOnVisableRepos(IReadOnlyList<RepoData> ListRepos)
    {
        if(ListRepos.Count == 0)
        {
            return (0, 0, 0, 0);
        }

        // Perform calculations off UI thread
        double totalLines = 0;
        double coveredLines = 0;
        double sumLineCoverage = 0;
        double sumBranchCoverage = 0;

        foreach (var r in ListRepos)
        {
            totalLines += r.TotalLines;
            coveredLines += r.CoveredLines;

            sumLineCoverage += r.CoveragePercent;
            sumBranchCoverage += r.BranchRate;
        }

        double count = ListRepos.Count;

        double AverageLineCoveragePercent = count > 0 ? sumLineCoverage / count : 0;

        double AverageBranchCoveragePercent = count > 0 ? sumBranchCoverage / count : 0;

        //Return processed data back to UI thread
        return (totalLines, coveredLines, AverageLineCoveragePercent, AverageBranchCoveragePercent);
    }

    [RelayCommand]
    public async Task GoToRepoPageAsync(RepoData repoData)
    {
        if (repoData is null)
        {
            return;
        }
        if (IsBusy)
        {
            return;
        }
        IsBusy = true;
        try
        {
            await Shell.Current.GoToAsync(nameof(RepoPage), true, new Dictionary<string, object>
            {
                { "SelectedRepo", repoData }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error navigating to RepoPage: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task GoToStaticDashboardPage()
    {
        if (IsBusy)
        {
            return;
        }
        IsBusy = true;
        try
        {
            await Shell.Current.GoToAsync($"//{nameof(StaticDashboardPage)}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error navigating to StaticDashboardPage: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ClearMemory()
    {
        if (datahandlerService.ClearMemory())
        {
            VisableData = null;
            LatestData = null;

            OnPropertyChanged(nameof(VisableData));

            await Shell.Current.ShowPopupAsync(new Label { Text = "Memory cleared successfully." });
        }
        else
        {
            await Shell.Current.ShowPopupAsync(new Label { Text = "No memory to clear or something went wrong." });
        }
    }
}
