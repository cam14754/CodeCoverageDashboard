// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.ViewModels;

public partial class StaticDashboardPageViewModel : BaseViewModel
{

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