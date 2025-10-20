// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Models;
public class MethodData
{
	public ClassData ParentClassData { get; set; }
	public Guid ParentID => ParentClassData.ParentID;
	public string? Name { get; set; } = "Unknown Name";
	public string[]? Errors { get; set; } = ["Unknown Errors"];
	public double? CoveragePercent { get; set; } = null;
	public double? CoveredLines { get; set; } = null;
}
