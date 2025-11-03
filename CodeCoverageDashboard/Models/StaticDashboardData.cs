// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.
namespace CodeCoverageDashboard.Models;

public partial class StaticDashboardData : ObservableObject
{
	//Calculated properties
	[NotifyPropertyChangedFor(nameof(TotalLinesUncoveredCount))]
	[ObservableProperty] public partial double TotalLinesCoveredCount { get; set; } = 0; //
	[ObservableProperty] public partial double TotalReposCount { get; set; } = 0; //
	[ObservableProperty] public partial double TotalClassesCount { get; set; } = 0; //
	[NotifyPropertyChangedFor(nameof(AverageComplexMethodPercent))]
	[ObservableProperty] public partial double TotalMethodsCount { get; set; } = 0; //
	[ObservableProperty] public partial double TotalLinesCount { get; set; } = 0; //
	[ObservableProperty] public partial double AverageBranchCoveragePercent { get; set; } = 0; //
	[ObservableProperty] public partial double AverageLineCoveragePercent { get; set; } = 0; //
	[ObservableProperty] public partial double TotalBracnhesCoveredCount { get; set; } = 0; //
	


	//Maybe Temp?
	public ObservableCollection<RepoData> ListRepos { get; set; } = []; //
	public ObservableCollection<MethodData> ListMethods { get; set; } = []; //


	//Given properties
	[ObservableProperty] public partial DateTime DateRetrieved { get; set; } = DateTime.MinValue; //
	[ObservableProperty] public partial string CoverletVersion { get; set; } = string.Empty; //
	[ObservableProperty] public partial string DashboardVersion { get; set; } = string.Empty; //

	// Calculated inferred properties
	//public int TotalRepos => ListRepos.Count;
	public double TotalLinesUncoveredCount => TotalLinesCount - TotalLinesCoveredCount; //	
	public double AverageComplexMethodPercent => ListMethods.Count / TotalMethodsCount; //
}
