// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Models;
public class MethodData
{
	public string? Name { get; set; } = "Unknown Name";
	public string[]? Errors { get; set; } = [];
	public double? CoveragePercent { get; set; } = null;
	public List<LineData> ListLines { get; set; } = [];
	public string? Signature { get; set; } = "()";
}