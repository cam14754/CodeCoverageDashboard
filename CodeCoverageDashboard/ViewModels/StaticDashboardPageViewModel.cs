// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.ViewModels;

public partial class StaticDashboardPageViewModel(IDatabaseService databaseService) : BaseViewModel
{
	readonly IDatabaseService databaseService = databaseService;
	readonly StaticDashboardData data = new();
	readonly StaticDashboardData twoweekolddata = new();


	public async Task GetLatestRepos()
	{
		if (IsBusy)
		{
			return;
		}
		IsBusy = true;
		try
		{
			var repos = await databaseService.LoadLatestReposList();
			data.ListRepos.Clear();
			foreach (var repo in repos)
			{
				data.ListRepos.Add(repo);
			}

			PopulateCalculatedFields();
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

	public void PopulateCalculatedFields()
	{
		//Populate ListMethods, TotalReposCount, TotalClassesCount, TotalMethodsCount, TotalLinesCount, TotalLinesCovered, TotalLinesUncovered
		foreach (var repo in data.ListRepos)
		{
			data.TotalReposCount++;
			foreach (var _class in repo.ListClasses)
			{
				data.TotalClassesCount++;
				foreach (var method in _class.ListMethods)
				{
					data.TotalMethodsCount++;
					data.ListMethods.Add(method);
					foreach (var line in method.ListLines)
					{
						data.TotalLinesCount++;
						if (line.Hits > 0)
						{
							data.TotalLinesCoveredCount++;
						}
						else
						{
							data.TotalLinesUncoveredCount++;
						}
					}
				}
			}
		}

		//Populate HotRepos

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