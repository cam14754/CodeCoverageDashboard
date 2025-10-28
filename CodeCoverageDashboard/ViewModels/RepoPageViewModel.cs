// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using CodeCoverageDashboard.Pages;

namespace CodeCoverageDashboard.ViewModels;
[QueryProperty(nameof(SelectedRepo), nameof(SelectedRepo))]
public partial class RepoPageViewModel : BaseViewModel
{
	[ObservableProperty]
	public partial RepoData SelectedRepo { get; set; }

	partial void OnSelectedRepoChanged(RepoData value)
	{
		Title = value?.Name ?? "Repository Details";
	}

	[RelayCommand]
	public async Task GoToClassPageAsync(ClassData classData)
	{
		if (classData is null)
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
			await Shell.Current.GoToAsync(nameof(ClassPage), true, new Dictionary<string, object>
			{
				{ "SelectedClass", classData }
			});
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error navigating to ClassPage: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}
}