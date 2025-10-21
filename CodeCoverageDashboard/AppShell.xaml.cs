// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(Pages.RepoPage), typeof(Pages.RepoPage));
		Routing.RegisterRoute(nameof(Pages.ClassPage), typeof(Pages.ClassPage));
	}
}
