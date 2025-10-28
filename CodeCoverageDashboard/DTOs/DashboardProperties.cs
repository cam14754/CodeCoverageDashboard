// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.DTOs;

public class DashboardProperties
{
	[JsonPropertyName("DateRetrieved")]
	public DateTime DateRetrieved { get; set; } = DateTime.MinValue;

	[JsonPropertyName("TotalCoveredLines")]
	public double TotalCoveredLines { get; set; } = 0;

	[JsonPropertyName("AverageCoveragePercent")]
	public double AverageCoveragePercent { get; set; } = 0;

	[JsonPropertyName("TotalNumLines")]
	public double TotalNumLines { get; set; } = 0;

	[JsonPropertyName("TotalUncoveredLines")]
	public double TotalUncoveredLines { get; set; } = 0;
}