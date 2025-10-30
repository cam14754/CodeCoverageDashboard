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
			Data.Add(PopulateCalculatedFields(currentReposDataList));

			//Get the past 5 dashboard data points from the database, for every 2 weeks
			for (int i = 1; i < 6; i++)
			{
				Data.Add(await databaseService.LoadXWeekOldDashboardData(i));
				if (Data[i] is null)
				{
					Data[i] = new StaticDashboardData();
					Data[i].DateRetrieved = DateTime.Now.AddDays(-14 * i);
				}
			}
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

	public static StaticDashboardData PopulateCalculatedFields(List<RepoData> repoDatas)
	{
		var dashboardData = new StaticDashboardData();
		dashboardData.TotalReposCount = repoDatas.Count;
		
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

		dashboardData.AverageLineCoveragePercent = AverageCoveragePercentSum / repoDatas.Count;

		return dashboardData;
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