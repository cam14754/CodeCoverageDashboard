// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.ObjectModel;

namespace CodeCoverageDashboard.ViewModels;
public partial class MainPageViewModel(IDataHandlerService dataHandlerService) : BaseViewModel
{
	public ObservableCollection<RepoData> Repos => dataHandlerService.Repos;

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

			dataHandlerService.LoadReposAsync();

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
			IsBusy = false;
		}
	}
}