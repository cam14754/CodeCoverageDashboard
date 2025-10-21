// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.ViewModels;

[QueryProperty(nameof(SelectedMethod), nameof(SelectedMethod))]
public partial class MethodPageViewModel : BaseViewModel
{
	[ObservableProperty]
	public partial MethodData SelectedMethod { get; set; }

	partial void OnSelectedMethodChanged(MethodData value)
	{
		Title = value?.Name ?? "Method Details";
	}
}
