// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.ViewModels;
public partial class MainPageViewModel(IDataHandlerService dataHandlerService) : BaseViewModel
{
	public ObservableCollection<RepoData> Repos => dataHandlerService.Repos;

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

			await dataHandlerService.GetXDocRequest();

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