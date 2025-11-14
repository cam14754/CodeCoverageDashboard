// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.ViewModels;

public partial class StaticDashboardPageViewModel(IDatabaseService databaseService, IDataHandlerService dataHandlerService) : BaseViewModel
{
    readonly IDatabaseService databaseService = databaseService;
    readonly IDataHandlerService dataHandlerService = dataHandlerService;

    // Stores 6 Data points. Get the past 12 weeks of data, every 2 weeks.
    public ObservableCollection<StaticDashboardData> Data { get; set; } = [];
    public StaticDashboardData? LatestData { get; set; }
    public StaticDashboardData? WeekoldData { get; set; }

    ObservableCollection<RepoData> currentReposDataList => dataHandlerService.Repos;

    public ObservableCollection<Tuple<MethodData, RepoData>> Top5Complex { get; set; } = [];
    public ObservableCollection<RepoData> Top5Healthy { get; set; } = [];
    public ObservableCollection<RepoData> Top5Unhealthy { get; set; } = [];
    public ObservableCollection<RepoData> Top5Hottest { get; set; } = [];

    public const string DashboardVersion = "0.3.5";

    [RelayCommand]
    public async Task GetRepoData(bool saveToDatabase = false)
    {
        if (IsBusy)
        {
            return;
        }
        IsBusy = true;
        try
        {
            await Execute(saveToDatabase);
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

    public async Task Execute(bool saveToDatabase = false)
    {
        
            // Replace instead of Clear to reduce churn
            Data = new ObservableCollection<StaticDashboardData>();
            OnPropertyChanged(nameof(Data));

            Debug.WriteLine("Starting calls...");

            // Start both operations. Offload DB load if it is synchronous behind the scenes.
            var getRepoDataTask = dataHandlerService.ProcessXDocsFromHTTP();
            var allDataTask = Task.Run(() => databaseService.LoadAllDashboardData());

            Debug.WriteLine("Awaiting calls...");
            await Task.WhenAll(getRepoDataTask, allDataTask).ConfigureAwait(true);

            var allData = allDataTask.Result;

            // Snapshot AFTER data fetch completes (UI thread)
            var reposSnapshot = dataHandlerService.Repos.ToList();
            OnPropertyChanged(nameof(currentReposDataList));

            // Heavy compute off UI thread using snapshots only
            var computeResult = await Task.Run(() =>
            {
                var latest = PopulateCalculatedFields(new ObservableCollection<RepoData>(reposSnapshot));

                var dashboards = new List<StaticDashboardData> { latest };
                dashboards.AddRange(allData);

                var oneWeekAgoDate = DateTime.Today.AddDays(-7).Date;
                var weekOld = dashboards.FirstOrDefault(d => d.DataAge.Date == oneWeekAgoDate)
                              ?? new StaticDashboardData();

                PopulateChangesFields(latest, weekOld);

                var topMethods = latest.ListRepos
                    .SelectMany(r => r.ListClasses.SelectMany(c => c.ListMethods.Select(m => new Tuple<MethodData, RepoData>(m, r))))
                    .OrderByDescending(x => x.Item1.Complexity)
                    .Take(5)
                    .ToList();

                var topHealthy = latest.ListRepos
                    .OrderByDescending(r => r.CoveragePercent)
                    .Take(5)
                    .ToList();

                var topUnhealthy = latest.ListRepos
                    .OrderBy(r => r.CoveragePercent)
                    .Take(5)
                    .ToList();

                var topHottest = latest.ListRepos
                    .OrderByDescending(r => r.CoveredLinesIncrease)
                    .Take(5)
                    .ToList();

                return new
                {
                    Latest = latest,
                    WeekOld = weekOld,
                    Dashboards = dashboards,
                    TopMethods = topMethods,
                    TopHealthy = topHealthy,
                    TopUnhealthy = topUnhealthy,
                    TopHottest = topHottest
                };
            }).ConfigureAwait(true);

            // UI thread updates
            LatestData = computeResult.Latest;
            WeekoldData = computeResult.WeekOld;
            Data = new ObservableCollection<StaticDashboardData>(computeResult.Dashboards);
            Top5Complex = new ObservableCollection<Tuple<MethodData, RepoData>>(computeResult.TopMethods);
            Top5Healthy = new ObservableCollection<RepoData>(computeResult.TopHealthy);
            Top5Unhealthy = new ObservableCollection<RepoData>(computeResult.TopUnhealthy);
            Top5Hottest = new ObservableCollection<RepoData>(computeResult.TopHottest);

            OnPropertyChanged(nameof(Data));
            OnPropertyChanged(nameof(LatestData));
            OnPropertyChanged(nameof(WeekoldData));
            OnPropertyChanged(nameof(Top5Complex));
            OnPropertyChanged(nameof(Top5Healthy));
            OnPropertyChanged(nameof(Top5Unhealthy));
            OnPropertyChanged(nameof(Top5Hottest));

            if (saveToDatabase && LatestData is not null)
            {
                // Offload save if implementation is synchronous
                await Task.Run(() => databaseService.SaveMemoryToDB(LatestData)).ConfigureAwait(true);
            }

            Debug.WriteLine("Execute finished (UI applied).");
    }

    public static StaticDashboardData PopulateCalculatedFields(ObservableCollection<RepoData> repoDatas)
    {
        Debug.WriteLine("Updating meta data...");

        var dashboardData = new StaticDashboardData();
        dashboardData.TotalReposCount = repoDatas.Count;
        dashboardData.DateRetrieved = DateTime.Now;
        //Dynamically Alocated
        dashboardData.CoverletVersion = "6.0.2";
        dashboardData.DashboardVersion = DashboardVersion;
        dashboardData.DataAge = repoDatas[0].DateRetrieved;

        double AverageCoveragePercentSum = 0;
        double AverageBranchCoveragePercentSum = 0;

        Debug.WriteLine("Successfully updated meta data...");

        Debug.WriteLine("Updating repo data...");

        
            foreach (RepoData repo in repoDatas)
            {
                dashboardData.ListRepos.Add(repo);
                AverageCoveragePercentSum += repo.CoveragePercent;
                AverageBranchCoveragePercentSum += repo.BranchRate;
                dashboardData.TotalBracnhesCoveredCount += repo.TotalCoveredBranches;

                foreach (ClassData classData in repo.ListClasses)
                {
                    dashboardData.TotalClassesCount++;
                    foreach (MethodData methodData in classData.ListMethods)
                    {
                        dashboardData.ListMethods.Add(methodData);

                        dashboardData.TotalMethodsCount++;
                        foreach (LineData lineData in methodData.ListLines)
                        {
                            dashboardData.TotalLinesCount++;
                            if (lineData.Hits >= 1)
                            {
                                dashboardData.TotalLinesCoveredCount++;
                            }
                        }
                    }
                }
            }
        

        dashboardData.AverageLineCoveragePercent = AverageCoveragePercentSum / repoDatas.Count;
        dashboardData.AverageBranchCoveragePercent = AverageBranchCoveragePercentSum / repoDatas.Count;

        Debug.WriteLine("Successfully updated repo data...");

        return dashboardData;
    }

    public static void PopulateChangesFields(StaticDashboardData latestData, StaticDashboardData previousData)
    {
        if (latestData is null)
        {
            throw new ArgumentNullException(nameof(latestData));
        }

        // If there's no previous data, treat as "all new"
        previousData ??= new StaticDashboardData();

        // Guard against null repo lists
        previousData.ListRepos ??= new ObservableCollection<RepoData>();
        latestData.ListRepos ??= new ObservableCollection<RepoData>();

        Debug.WriteLine("Updating changes fields...");

        var previousByName = previousData.ListRepos
            .Where(r => !string.IsNullOrWhiteSpace(r.Name))
            .ToDictionary(r => r.Name, r => r);

        foreach (var latestRepo in latestData.ListRepos)
        {
            if (latestRepo is null)
            {
                continue;
            }

            if (!previousByName.TryGetValue(latestRepo.Name, out var match) || match is null)
            {
                //If not match found, increase is just current
                latestRepo.CoveredLinesIncrease = latestRepo.CoveredLines;

                latestRepo.CoveragePercentPercentIncrease = latestRepo.CoveragePercent;

            } 
            else
            {
                // Compute line delta
                latestRepo.CoveredLinesIncrease = latestRepo.CoveredLines - match.CoveredLines;

                latestRepo.CoveragePercentPercentIncrease = latestRepo.CoveragePercent - match.CoveragePercent;
            }
        }

        Debug.WriteLine("Successfully updated changes fields.");
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