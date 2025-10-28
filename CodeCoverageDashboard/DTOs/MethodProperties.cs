// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.DTOs;

public class MethodProperties
{
	[JsonPropertyName("ClassName")]
	public string ClassName { get; set; } = string.Empty;

	[JsonPropertyName("MethodName")]
	public string MethodName { get; set; } = string.Empty;

	[JsonPropertyName("Signature")]
	public string Signature { get; set; } = string.Empty;

	[JsonPropertyName("CoveragePercent")]
	public double CoveragePercent { get; set; } = 0;

	[JsonPropertyName("Lines")]
	public List<LineData> Lines { get; set; } = [];
}