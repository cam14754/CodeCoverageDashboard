// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.ViewModels;
public partial class MainPageViewModel(IRepoDataService repoDataService) : BaseViewModel
{

	[RelayCommand]
	public async Task LoadReposAsync()
	{
		if (IsBusy)
		{
			return;
		}
		try
		{
			IsBusy = true;
			Debug.WriteLine("Loading repos...");
			if (!await repoDataService.GetRepoDataAsync())
			{
				throw new Exception("Failed to load repo data.");
			}

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
}