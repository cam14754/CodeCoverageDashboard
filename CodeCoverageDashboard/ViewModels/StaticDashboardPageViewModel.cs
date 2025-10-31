// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.ViewModels;

public partial class StaticDashboardPageViewModel(IDatabaseService databaseService) : BaseViewModel
{
	readonly IDatabaseService databaseService = databaseService;

	// Stores 6 Data points. Get the past 12 weeks of data, every 2 weeks.
	public ObservableCollection<StaticDashboardData> Data { get; set; } = [];
	public StaticDashboardData? LatestData => Data.FirstOrDefault();

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
			StaticDashboardData y = PopulateCalculatedFields(currentReposDataList);

			Data.Add(y);

			//Get the past 5 dashboard data points from the database, for every 2 weeks
			//Simulate data for now
			for (int i = 1; i < 6; i++)
			{
				var x = await databaseService.LoadXWeekOldDashboardData(i);
				if (x is null)
				{
					Random rnd = new Random();
					x = new StaticDashboardData();
					x.DateRetrieved = DateTime.Now.AddDays(-14 * i);
					x.TotalLinesCoveredCount = (Data[0].TotalLinesCoveredCount - 100 * i * (double)rnd.Next(0, 100)/100);
					x.AverageLineCoveragePercent = (Data[0].AverageLineCoveragePercent - 0.01 * i * (double)rnd.Next(0, 100) / 100);
					x.ListRepos = y.ListRepos;
					foreach(RepoData repo in x.ListRepos)
					{
						repo.CoveragePercent -= 0.01 * i * (double)rnd.Next(0, 100) / 100;
					}
				}
				Data.Add(x);
			}

			//Calculate changes with current data
			PopulateChangesFields(y);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading latest repos: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
			OnPropertyChanged(nameof(LatestData));
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

		foreach (RepoData repo in repoDatas)
		{
			dashboardData.ListRepos.Add(repo);
			AverageCoveragePercentSum += repo.CoveragePercent;
			if(repo.CoveragePercent < 0.80)
			{
				dashboardData.UnhealthyRepos.Add(repo);
			}
			else
			{
				dashboardData.HealthyRepos.Add(repo);
			}

			foreach (ClassData classData in repo.ListClasses)
			{
				dashboardData.TotalClassesCount++;
				foreach (MethodData methodData in classData.ListMethods)
				{
					if (methodData.Complexity > 10)
					{
						dashboardData.ComplexMethods.Add(methodData);
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

		var topComplexMethods = dashboardData.ComplexMethods.OrderByDescending(x => x.Complexity).Take(5).ToList();
		dashboardData.ComplexMethods.Clear();
		foreach (var method in topComplexMethods)
		{
			dashboardData.ComplexMethods.Add(method);
		}

		dashboardData.AverageLineCoveragePercent = AverageCoveragePercentSum / repoDatas.Count;

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

		dashboardData.HotRepos = new ObservableCollection<RepoData>(list.OrderByDescending(x => x.change).Take(5).Select(x => x.repo));
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