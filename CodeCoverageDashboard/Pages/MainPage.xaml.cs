// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using CodeCoverageDashboard.ViewModels;

namespace CodeCoverageDashboard;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel mainPageViewModel)
	{
		BindingContext = mainPageViewModel;
		InitializeComponent();
	}
}