// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Models;
public class ClassData
{
	public RepoData ParentRepoData { get; set; } = null!;
	public Guid ParentID => ParentRepoData.ID;
	public string? Name { get; set; } = "Unknown Name";
	public string[]? Errors { get; set; } = [];
	public double? CoveragePercent { get; set; } = null;
	public List<MethodData>? ListMethods { get; set; } = null;
}
