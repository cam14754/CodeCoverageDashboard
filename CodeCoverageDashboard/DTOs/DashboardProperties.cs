// COPYRIGHT © 2025 ESRI
//
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States.
//
// For additional information, contact:
// Environmental Systems Research Institute, Inc.
// Attn: Contracts Dept
// 380 New York Street
// Redlands, California, USA 92373
//
// email: contracts@esri.com

namespace CodeCoverageDashboard.DTOs;

public class DashboardProperties
{
    [JsonPropertyName("TotalLinesCoveredCount")] public double TotalLinesCoveredCount { get; set; } = 0;

    [JsonPropertyName("TotalReposCount")] public double TotalReposCount { get; set; } = 0;
    [JsonPropertyName("TotalClassesCount")] public double TotalClassesCount { get; set; } = 0;
    [JsonPropertyName("TotalMethodsCount")] public double TotalMethodsCount { get; set; } = 0;
    [JsonPropertyName("TotalLinesCount")] public double TotalLinesCount { get; set; } = 0;
    [JsonPropertyName("AverageLineCoveragePercent")] public double AverageLineCoveragePercent { get; set; } = 0;
    [JsonPropertyName("AverageBranchCoveragePercent")] public double AverageBranchCoveragePercent { get; set; } = 0;
    [JsonPropertyName("TotalBranchesCoveredCount")] public double TotalBranchesCoveredCount { get; set; } = 0;
    [JsonPropertyName("AverageComplexMethodPercent")] public double AverageComplexMethodPercent { get; set; } = 0;
    [JsonPropertyName("HotRepos")] public ObservableCollection<RepoData> HotRepos { get; set; } = [];
    [JsonPropertyName("ComplexMethods")] public ObservableCollection<MethodData> ComplexMethods { get; set; } = [];
    [JsonPropertyName("HealthyRepos")] public ObservableCollection<RepoData> HealthyRepos { get; set; } = [];
    [JsonPropertyName("UnhealthyRepos")] public ObservableCollection<RepoData> UnhealthyRepos { get; set; } = [];

    //Given properties
    [JsonPropertyName("DateRetrieved")] public DateTime DateRetrieved { get; set; } = DateTime.MinValue;
    [JsonPropertyName("DataAge")] public DateTime DataAge { get; set; } = DateTime.MinValue;

    [JsonPropertyName("CoverletVersion")] public string CoverletVersion { get; set; } = string.Empty;
    [JsonPropertyName("DashboardVersion")] public string DashboardVersion { get; set; } = string.Empty;
    [JsonPropertyName("ListRepos")] public ObservableCollection<RepoData> ListRepos { get; set; } = [];

    [JsonPropertyName("TotalLinesUncoveredCount")] public double TotalLinesUncoveredCount { get; set; } = 0;
    [JsonPropertyName("TotalComplexMethodCount")] public double TotalComplexMethodCount { get; set; } = 0;
}
