// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.DTOs;

public class RepoProperties
{
	[JsonPropertyName("RepoName")]
	public string RepoName { get; set; } = string.Empty;

	[JsonPropertyName("DateRetrieved")]
	public DateTime DateRetrieved { get; set; } = DateTime.MinValue;

	[JsonPropertyName("CoveredLines")]
	public double CoveredLines { get; set; } = 0;

	[JsonPropertyName("NumLines")]
	public double NumLines { get; set; } = 0;

	[JsonPropertyName("CoveragePercent")]
	public double CoveragePercent { get; set; } = 0;

	[JsonPropertyName("UncoveredLines")]
	public double UncoveredLines { get; set; } = 0;

	[JsonPropertyName("Classes")]
	public List<ClassData> Classes { get; set; } = [];
}