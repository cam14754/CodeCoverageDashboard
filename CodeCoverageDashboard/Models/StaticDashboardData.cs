// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Models;
public partial class StaticDashboardData : ObservableObject
{
	[ObservableProperty] public partial double TotalLines { get; set; } = 0;
	[ObservableProperty] public partial double TotalCoveredLines { get; set; } = 0;
	[ObservableProperty] public partial double AverageCoverage { get; set; } = 0;
	[ObservableProperty] public partial double AverageBranchCoverage { get; set; } = 0;
	[ObservableProperty] public partial double TotalRepos { get; set; } = 0;
	[ObservableProperty] public partial double TotalClasses { get; set; } = 0;
	[ObservableProperty] public partial double TotalMethods { get; set; } = 0;
	[ObservableProperty] public partial DateTime DateRetrieved { get; set; } = DateTime.MinValue;

	public ObservableCollection<RepoData> HotRepos { get; set; } = [];
	public ObservableCollection<RepoData> ComplexMethods { get; set; } = [];
}
