// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using CodeCoverageDashboard.Pages;

namespace CodeCoverageDashboard.ViewModels;

[QueryProperty(nameof(SelectedClass), nameof(SelectedClass))]
public partial class ClassPageViewModel : BaseViewModel
{
	[ObservableProperty]
	public partial ClassData SelectedClass { get; set; }

	partial void OnSelectedClassChanged(ClassData value)
	{
		Title = value?.Name ?? "Class Details";
	}

	[RelayCommand]
	public async Task GoToMethodPageAsync(MethodData methodData)
	{
		if (methodData is null)
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
			await Shell.Current.GoToAsync(nameof(MethodPage), true, new Dictionary<string, object>
			{
				{ "SelectedMethod", methodData }
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
