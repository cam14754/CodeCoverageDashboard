// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.ViewModels;

public partial class BaseViewModel : ObservableObject
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsNotBusy))]
	bool isBusy;

	[ObservableProperty]
	string? title;

	[ObservableProperty]
	bool isRefreshing;

	public bool IsNotBusy => !isBusy;
}
