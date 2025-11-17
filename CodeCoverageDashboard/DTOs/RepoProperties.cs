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

public class RepoProperties
{
    [JsonPropertyName("RepoName")]
    public string RepoName { get; set; } = string.Empty;

    [JsonPropertyName("DateRetrieved")]
    public DateTime DateRetrieved { get; set; } = DateTime.MinValue;

    [JsonPropertyName("CoveredLines")]
    public double CoveredLines { get; set; } = 0;
    [JsonPropertyName("CoveredLinesIncrease")]
    public double CoveredLinesIncrease { get; set; } = 0;

    [JsonPropertyName("NumLines")]
    public double NumLines { get; set; } = 0;

    [JsonPropertyName("CoveragePercent")]
    public double CoveragePercent { get; set; } = 0;
    [JsonPropertyName("CoveragePercentPercentIncrease")]
    public double CoveragePercentPercentIncrease { get; set; } = 0;

    [JsonPropertyName("UncoveredLines")]
    public double UncoveredLines { get; set; } = 0;
    [JsonPropertyName("TotalBranches")]
    public double TotalBranches { get; set; } = 0;
    [JsonPropertyName("TotalCoveredBranches")]
    public double TotalCoveredBranches { get; set; } = 0;

    [JsonPropertyName("BranchRate")]
    public double BranchRate { get; set; } = 0;

    [JsonPropertyName("Classes")]
    public List<ClassData> Classes { get; set; } = [];

}
