// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace CodeCoverageDashboard.ViewModels;

public partial class StaticDashboardPageViewModel(IDatabaseService databaseService) : BaseViewModel
{
	readonly IDatabaseService databaseService = databaseService;

	// Stores 6 Data points. Get the past 12 weeks of data, every 2 weeks.
	public ObservableCollection<StaticDashboardData> Data { get; set; } = [];
	public StaticDashboardData? LatestData { get; set; }
	public StaticDashboardData? WeekoldData { get; set; }

	public ObservableCollection<Tuple<MethodData, RepoData>> Top5Complex { get; set; } = [];
	public ObservableCollection<RepoData> Top5Healthy { get; set; } = [];
	public ObservableCollection<RepoData> Top5Unhealthy { get; set; } = [];
	public ObservableCollection<RepoData> Top5Hottest { get; set; } = [];

	public const string DashboardVersion = "0.3.1";

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
			Data.Clear();

			//Get the list of latest repos from the database
			var currentReposDataList = await databaseService.LoadLatestReposList();

			//Calculate the dashboard data from the current repos, and add to memory collection
			LatestData = PopulateCalculatedFields(currentReposDataList);
			Data.Add(LatestData);

			WeekoldData = await databaseService.LoadXWeekOldDashboardData(1);
			if(WeekoldData is null)
			{
				Debug.WriteLine("No week old data found in database.");
				WeekoldData = new StaticDashboardData();
			}

			var allData = await databaseService.LoadAllDashboardData();
			foreach (var dash in allData.OrderByDescending(d => d.DateRetrieved))
			{
				Data.Add(dash);
			}

			PopulateChangesFields(LatestData, WeekoldData);
			
			UpdateTop5s();

			OnPropertyChanged(nameof(LatestData));
			OnPropertyChanged(nameof(WeekoldData));
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

	void UpdateTop5s()
	{
		if(LatestData is null)
		{
			return;
		}

		Top5Complex.Clear();
		var topMethods = LatestData.ListRepos
			.SelectMany(r => r.ListClasses
				.SelectMany(c => c.ListMethods
					.Select(m => new { Repo = r, Method = m })))
		.OrderByDescending(x => x.Method.Complexity)
		.Take(5);

		foreach (var item in topMethods)
		{
			Top5Complex.Add(new Tuple<MethodData, RepoData>(item.Method, item.Repo));
		}

		Top5Healthy.Clear();
		foreach (var r in LatestData.ListRepos.OrderByDescending(r => r.CoveragePercent).Take(5))
		{
			Top5Healthy.Add(r);
		}

		Top5Unhealthy.Clear();
		foreach (var r in LatestData.ListRepos.OrderBy(r => r.CoveragePercent).Take(5))
		{
			Top5Unhealthy.Add(r);
		}

		Top5Hottest.Clear();
		foreach (var r in LatestData.ListRepos.OrderByDescending(r => r.CoveragePercentPercentIncrease).Take(5))
		{
			Top5Hottest.Add(r);
		}
	}

	public static StaticDashboardData PopulateCalculatedFields(List<RepoData> repoDatas)
	{
		var dashboardData = new StaticDashboardData();
		dashboardData.TotalReposCount = repoDatas.Count;
		dashboardData.DateRetrieved = DateTime.Now;
		dashboardData.CoverletVersion = "6.0.2";
		dashboardData.DashboardVersion = DashboardVersion;

		double AverageCoveragePercentSum = 0;
		double AverageBranchCoveragePercentSum = 0;

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

		return dashboardData;
	}

	public static void PopulateChangesFields(StaticDashboardData latestData, StaticDashboardData PreviousData)
	{
		foreach (var latestRepo in latestData.ListRepos)
		{
			RepoData match = PreviousData.ListRepos.FirstOrDefault(r => r.Name == latestRepo.Name);

			if (match is null)
			{
				Debug.WriteLine($"No matching repo found for {latestRepo.Name} in second latest data.");
				latestRepo.CoveredLinesIncrease = latestRepo.CoveredLines;
				if (latestRepo.CoveredLinesIncrease <= 0)
				{
					latestRepo.CoveragePercentPercentIncrease = 0;
				} else
				{
					latestRepo.CoveragePercentPercentIncrease = latestRepo.CoveragePercent;
				}
				continue;
			}

			latestRepo.CoveredLinesIncrease = latestRepo.CoveredLines - match.CoveredLines;

			if (latestRepo.CoveredLinesIncrease <= 0) 
			{
				latestRepo.CoveragePercentPercentIncrease = 0;
			}
			else
			{
				latestRepo.CoveragePercentPercentIncrease = latestRepo.CoveragePercent - match.CoveragePercent;
			}
		}
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
			await Shell.Current.GoToAsync($"//{nameof(MainPage)}", true);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error navigating to MainPage: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}
}