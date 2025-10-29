// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.DTOs;

public class DashboardProperties
{
	[JsonPropertyName("TotalLinesCovered")] public double TotalLinesCovered { get; set; } = 0;
	[JsonPropertyName("TotalReposCount")] public double TotalReposCount { get; set; } = 0;
	[JsonPropertyName("TotalClassesCount")] public double TotalClassesCount { get; set; } = 0;
	[JsonPropertyName("TotalMethodsCount")] public double TotalMethodsCount { get; set; } = 0;
	[JsonPropertyName("TotalLinesCount")] public double TotalLinesCount { get; set; } = 0;
	[JsonPropertyName("AverageLineCoveragePercent")] public double AverageLineCoveragePercent { get; set; } = 0;
	[JsonPropertyName("AverageBranchCoveragePercent")] public double AverageBranchCoveragePercent { get; set; } = 0;
	[JsonPropertyName("TotalBracnhesCoveredCount")] public double TotalBracnhesCoveredCount { get; set; } = 0;
	[JsonPropertyName("TotalComplexMethodsCount")] public double TotalComplexMethodsCount { get; set; } = 0;
	[JsonPropertyName("AverageComplexMethodPercent")] public double AverageComplexMethodPercent { get; set; } = 0;
	[JsonPropertyName("DateRetrieved")] public DateTime DateRetrieved { get; set; } = DateTime.MinValue;
	[JsonPropertyName("CoverletVersion")] public string CoverletVersion { get; set; } = string.Empty;
	[JsonPropertyName("DashboardVersion")] public string DashboardVersion { get; set; } = string.Empty;
	[JsonPropertyName("HotRepos")] public ObservableCollection<RepoData> HotRepos { get; set; } = [];
	[JsonPropertyName("ComplexMethods")] public ObservableCollection<MethodData> ComplexMethods { get; set; } = [];
	[JsonPropertyName("HealthyRepos")] public ObservableCollection<RepoData> HealthyRepos { get; set; } = [];
	[JsonPropertyName("UnhealthyRepos")] public ObservableCollection<RepoData> UnhealthyRepos { get; set; } = [];
}