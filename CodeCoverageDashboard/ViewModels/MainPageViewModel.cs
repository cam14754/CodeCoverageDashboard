// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.ObjectModel;
using CodeCoverageDashboard.Pages;

namespace CodeCoverageDashboard.ViewModels;
public partial class MainPageViewModel(IDataHandlerService dataHandlerService) : BaseViewModel
{
	public ObservableCollection<RepoData> Repos => dataHandlerService.Repos;
	public ObservableCollection<ClassData> Classes => classes;
	ObservableCollection<ClassData> classes = new(dataHandlerService.Repos.SelectMany(r => r.ListClasses));

	[ObservableProperty]
	public partial bool DeleteExistingReports { get; set; } = true;

	[RelayCommand]
	public void LoadRepos()
	{
		if (IsBusy)
		{
			return;
		}
		try
		{
			IsBusy = true;
			Debug.WriteLine("Loading repos... \n");

			dataHandlerService.LoadRepos();

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

	[RelayCommand]
	public async Task RefreshAsync()
	{
		if (IsBusy)
		{
			return;
		}
		IsBusy = true;
		try
		{

			if(DeleteExistingReports)
			{
				Debug.WriteLine("Deleting existing coverage reports...");
				// Path to the results directory
				string path = "C:\\Users\\cam14754\\Desktop\\TestReports";
				if (Directory.Exists(path))
				{
					Directory.Delete(path, true); // 'true' means recursive delete
					Debug.WriteLine("Existing coverage reports deleted.");
				}
			}

			Debug.WriteLine("");
			Debug.WriteLine("Begining Analysis...");
			await dataHandlerService.TestReposAsync();
			Debug.WriteLine("");
			Debug.WriteLine("Analysis Complete.");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error: {ex.Message}.");
		}
		finally
		{
			classes = new ObservableCollection<ClassData>(dataHandlerService.Repos.SelectMany(r => r.ListClasses));
			OnPropertyChanged(nameof(Classes));
			IsBusy = false;
		}
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
}