// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

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
}
