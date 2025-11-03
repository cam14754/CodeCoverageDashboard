// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.ObjectModel;

namespace CodeCoverageDashboard.ViewModels;

public partial class StaticDashboardPageViewModel(IDatabaseService databaseService) : BaseViewModel
{
	readonly IDatabaseService databaseService = databaseService;

	// Stores 6 Data points. Get the past 12 weeks of data, every 2 weeks.
	public ObservableCollection<StaticDashboardData> Data { get; set; } = [];
	public StaticDashboardData? LatestData { get; set; }

	public ObservableCollection<MethodData> Top5Complex { get; set; } = [];
	public ObservableCollection<RepoData> Top5Healthy { get; set; } = [];
	public ObservableCollection<RepoData> Top5Unhealthy { get; set; } = [];
	public ObservableCollection<RepoData> Top5Hottest { get; set; } = [];




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

			//Get the past 5 dashboard data points from the database, for every 2 weeks
			//Simulate data for now
			for (int i = 1; i < 6; i++)
			{
				var TempMockDashData = await databaseService.LoadXWeekOldDashboardData(i);
				if (TempMockDashData is null)
				{
					Random rnd = new Random();
					TempMockDashData = new StaticDashboardData();
					TempMockDashData.DateRetrieved = DateTime.Now.AddDays(-14 * i);
					TempMockDashData.TotalLinesCoveredCount = (Data[i - 1].TotalLinesCoveredCount - 100 * i * (double)rnd.Next(0, 500) / 100);
					TempMockDashData.AverageLineCoveragePercent = (Data[i - 1].AverageLineCoveragePercent - 0.01 * i * (double)rnd.Next(0, 100) / 100);
					TempMockDashData.ListRepos = LatestData.ListRepos;
					foreach (RepoData repo in TempMockDashData.ListRepos)
					{
						repo.CoveragePercent -= 0.01 * i * (double)rnd.Next(0, 100) / 100;
					}
				}
				Data.Add(TempMockDashData);
			}

			//Calculate changes with current data
			PopulateChangesFields(LatestData);

			UpdateTop5s();

			await databaseService.SaveMemoryToDB(LatestData);
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
		foreach (var m in LatestData.ListMethods.OrderByDescending(m => m.Complexity).Take(5))
		{
			Top5Complex.Add(m);
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
		dashboardData.DashboardVersion = "0.1";


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
					if (methodData.Complexity > 10)
					{
						dashboardData.ListMethods.Add(methodData);
					}
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

	public void PopulateChangesFields(StaticDashboardData dashboardData)
	{
		Random rnd = new Random();
		var list = new List<(RepoData repo, double change)>();

		foreach (RepoData repo in dashboardData.ListRepos)
		{
			repo.CoveragePercentPercentIncrease = rnd.Next(0, 100);
			list.Add((repo, repo.CoveragePercentPercentIncrease));
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