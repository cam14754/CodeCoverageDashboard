// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Models;
public sealed class RepoData
{
	public string? Url { get; set; } = "Unknown URL";
	public string? Name { get; set; } = "Unknown Name";
	public string? LocalPath { get; set; } = "Unknown Path";
	public string? Org { get; set; } = "Unkown Org";
	public bool? IsValid { get; set; } = false;
	public string? Error { get; set; } = "Unknown Errors";
	public DateTime DateRetrieved { get; set; } = DateTime.Now;

}
