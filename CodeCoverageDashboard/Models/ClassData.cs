// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Models;
public partial class ClassData : ObservableObject
{
	[ObservableProperty]
	public partial string? Name { get; set; } = "Unknown Name";
	[ObservableProperty]

	public partial string? Filename { get; set; } = "Unknown Filename";

	[ObservableProperty]

	public partial string[]? Errors { get; set; } = [];
	[ObservableProperty]

	public partial double? CoveragePercent { get; set; } = null;
	

	public List<MethodData> ListMethods { get; set; } = [];
}
