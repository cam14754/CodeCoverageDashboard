// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard;

public partial class MethodPage : ContentPage
{
	public MethodPage(MethodPageViewModel methodPageViewModel)
	{
		InitializeComponent();
		BindingContext = methodPageViewModel;

	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
	}
}