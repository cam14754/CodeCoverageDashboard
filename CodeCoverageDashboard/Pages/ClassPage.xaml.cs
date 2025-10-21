// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Pages;

public partial class ClassPage : ContentPage
{
	public ClassPage(ClassPageViewModel classPageViewModel)
	{
		InitializeComponent();
		BindingContext = classPageViewModel;

	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
	}
}