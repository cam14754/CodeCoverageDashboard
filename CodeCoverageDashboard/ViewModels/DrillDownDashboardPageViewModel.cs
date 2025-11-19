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
    public ObservableCollection<RepoData>? LatestRepos { get; set; }
    public ObservableCollection<RepoData>? VisableRepos { get; set; }
    public StaticDashboardData? LatestData { get; set; }
    public StaticDashboardData? VisableData { get; set; }

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

            await datahandlerService.LoadLatestStaticDashboardData();

            if (datahandlerService.Latest is null)
            {
                Debug.WriteLine("DB call failed");
                return;
            }

            if (datahandlerService.Latest.FirstOrDefault() is null)
            {
                Debug.WriteLine("DB call completed, but no repos loaded");
                return;
            }

            //Save all data to memory
            LatestData = datahandlerService.Latest.FirstOrDefault()!;
            VisableData = LatestData;
            LatestRepos = new ObservableCollection<RepoData>([.. LatestData.ListRepos.OrderBy(r => r.Name)]);
            VisableRepos = LatestRepos;

            CalculatePropertiesBasedOnCurrentVisableRepos(VisableRepos, VisableData);

            OnPropertyChanged(nameof(VisableRepos));
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

    public static void CalculatePropertiesBasedOnCurrentVisableRepos(ObservableCollection<RepoData> repos, StaticDashboardData dashs)
    {

        if (repos == null || repos.Count == 0)
        {
            // Reset values for empty view
            dashs.TotalLinesCount = 0;
            dashs.TotalLinesCoveredCount = 0;
            dashs.AverageLineCoveragePercent = 0;
            dashs.AverageBranchCoveragePercent = 0;
            return;
        }

        double totalLines = 0;
        double coveredLines = 0;
        double sumLineCoverage = 0;
        double sumBranchCoverage = 0;

        foreach (var r in repos)
        {
            totalLines += r.TotalLines;
            coveredLines += r.CoveredLines;

            sumLineCoverage += r.CoveragePercent;
            sumBranchCoverage += r.BranchRate;
        }

        double count = repos.Count;

        dashs.TotalLinesCount = totalLines;
        dashs.TotalLinesCoveredCount = coveredLines;

        dashs.AverageLineCoveragePercent =
            count > 0 ? sumLineCoverage / count : 0;

        dashs.AverageBranchCoveragePercent =
            count > 0 ? sumBranchCoverage / count : 0;
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
    public async Task ClearCache()
    {
        if (datahandlerService.ClearCache())
        {
            VisableRepos = null;
            VisableData = null;
            LatestRepos = null;
            LatestData = null;

            OnPropertyChanged(nameof(VisableRepos));
            OnPropertyChanged(nameof(VisableData));

            await Shell.Current.ShowPopupAsync(new Label { Text = "Cache cleared successfully." });
            
        }
        else
        {
            await Shell.Current.ShowPopupAsync(new Label { Text = "No cache to clear or something went wrong." });
        }
    }
}
