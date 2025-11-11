// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace CodeCoverageDashboard.ViewModels;

public partial class StaticDashboardPageViewModel(IDatabaseService databaseService, IDataHandlerService dataHandlerService) : BaseViewModel
{
	readonly IDatabaseService databaseService = databaseService;
	readonly IDataHandlerService dataHandlerService = dataHandlerService;

	// Stores 6 Data points. Get the past 12 weeks of data, every 2 weeks.
	public ObservableCollection<StaticDashboardData> Data { get; set; } = [];
	public StaticDashboardData? LatestData { get; set; }
	public StaticDashboardData? WeekoldData { get; set; }

	List<RepoData> currentReposDataList = [];

	public ObservableCollection<Tuple<MethodData, RepoData>> Top5Complex { get; set; } = [];
	public ObservableCollection<RepoData> Top5Healthy { get; set; } = [];
	public ObservableCollection<RepoData> Top5Unhealthy { get; set; } = [];
	public ObservableCollection<RepoData> Top5Hottest { get; set; } = [];

	public const string DashboardVersion = "0.3.4";

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
			Data.Clear();


			//Load and save from HTTP
			Debug.WriteLine("Loading latest repo data...");
			await dataHandlerService.ProcessXDocsFromHTTP();
			if(dataHandlerService.Repos.Count is 0 || dataHandlerService.Repos is null)
			{
				throw new Exception("Latest repo data is null after attempting to load.");
			}
			currentReposDataList = [.. dataHandlerService.Repos];
			Debug.WriteLine("Successfully loaded all repo data.");


			//Load and save all previous dashboard data from DB
			Debug.WriteLine("Loading all dashboard data...");
			var allData = await databaseService.LoadAllDashboardData();
			if(allData is null || allData.Count is 0)
			{
				throw new Exception("Latest dashboard data is null after attempting to load.");
			}
			foreach (var dash in allData.OrderByDescending(d => d.DateRetrieved))
			{
				Data.Add(dash);
			}
			Debug.WriteLine("Successfully loaded all dashboard data.");


			//Load week old data from DB
			Debug.WriteLine("Loading week old dashboard data...");
			WeekoldData = await databaseService.LoadXWeekOldDashboardData(1);
			if(WeekoldData is null)
			{
				Debug.WriteLine("No week old data found in database.");
				WeekoldData = new StaticDashboardData();
			}
			Debug.WriteLine("Successfully loaded week old dashboard data.");

			//Calculate the dashboard data from the current repos, and add to memory collection
			LatestData = await PopulateCalculatedFields(currentReposDataList);
			Data.Add(LatestData);

			//Calculate changes since last week
			PopulateChangesFields(LatestData, WeekoldData);

			//Update Top 5s
			UpdateTop5s();

			//Notify UI
			OnPropertyChanged(nameof(LatestData));
			OnPropertyChanged(nameof(WeekoldData));

			//Save latest data to DB
			if (saveToDatabase)
			{
				await databaseService.SaveMemoryToDB(LatestData);
			}

			Debug.WriteLine("\nFinished processing data.");
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
		Debug.WriteLine("Adding data to top 5's...");

		if (LatestData is null)
		{
			Debug.WriteLine("Latest data is null");
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
		Debug.WriteLine("Successfully updated top 5's");
	}

	public static async Task<StaticDashboardData> PopulateCalculatedFields(List<RepoData> repoDatas)
	{
		Debug.WriteLine("Updating meta data...");

		var dashboardData = new StaticDashboardData();
		dashboardData.TotalReposCount = repoDatas.Count;
		dashboardData.DateRetrieved = repoDatas[0].DateRetrieved;
		//Dynamically Alocated
		dashboardData.CoverletVersion = "6.0.2";
		dashboardData.DashboardVersion = DashboardVersion;
		dashboardData.DataAge = repoDatas.Max(r => r.DateRetrieved);

		double AverageCoveragePercentSum = 0;
		double AverageBranchCoveragePercentSum = 0;

		Debug.WriteLine("Successfully updated meta data...");

		Debug.WriteLine("Updating repo data...");

		await Task.Run(() =>
		{
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
		});

		dashboardData.AverageLineCoveragePercent = AverageCoveragePercentSum / repoDatas.Count;
		dashboardData.AverageBranchCoveragePercent = AverageBranchCoveragePercentSum / repoDatas.Count;

		Debug.WriteLine("Successfully updated repo data...");

		return dashboardData;
	}

	public static void PopulateChangesFields(StaticDashboardData latestData, StaticDashboardData PreviousData)
	{
		Debug.WriteLine("Updating chages fields...");
		foreach (var latestRepo in latestData.ListRepos)
		{
			RepoData match = PreviousData.ListRepos.FirstOrDefault(r => r.Name == latestRepo.Name);

			if (match is null)
			{
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