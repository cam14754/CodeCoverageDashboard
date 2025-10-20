// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace CodeCoverageDashboard.Models;
public partial class RepoData : ObservableObject
{

	[ObservableProperty]
	public partial Guid ID { get; set; } = Guid.NewGuid();

	[ObservableProperty]
	public partial string? AbsolutePath { get; set; } = "Unknown URL";

	[ObservableProperty]
	public partial string? Name { get; set; } = "Unknown Name";

	[ObservableProperty]
	public partial bool? IsValid { get; set; } = false;

	[ObservableProperty]
	private ObservableCollection<string> errors = [];

	[ObservableProperty]
	public partial double? CoveragePercent { get; set; } = null;

	[ObservableProperty]
	public partial double? CoveredLines { get; set; } = null;

	[ObservableProperty]
	public partial double? TotalLines { get; set; } = null;

	[ObservableProperty]
	public partial double? UncoveredLines { get; set; } = null;

	[ObservableProperty]
	public partial DateTime DateRetrieved { get; set; } = DateTime.Now;

	[ObservableProperty]
	public partial List<ClassData> ListClasses { get; set; } = null;

	[ObservableProperty]
	public partial XDocument XDocument { get; set; } = null;
}
