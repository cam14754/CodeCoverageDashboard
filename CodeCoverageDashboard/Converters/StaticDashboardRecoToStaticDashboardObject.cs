// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Converters;

public static class RepoRecordToRepoObject
{
	public static object? Convert(object? value)
	{
		var x = new RepoData();
		var repoRecord = (RepoRecord)value ?? throw new NullReferenceException("RepoRecord is null in RepoRecordToRepoObject converter.");

		x.Name = repoRecord.RepoName;
		x.DateRetrieved = repoRecord.DateRetrieved;

		var properties = repoRecord.Properties ?? throw new NullReferenceException("RepoRecord.Properties is null in RepoRecordToRepoObject converter.");

		x.CoveredLines = properties.CoveredLines;
		x.TotalLines = properties.NumLines;
		x.CoveragePercent = properties.CoveragePercent;
		x.UncoveredLines = properties.UncoveredLines;
		x.ListClasses = properties.Classes;

		return x;
	}

	public static object? ConvertBack(object? value)
	{
		var x = new RepoRecord();
		var repoData = (RepoData)value ?? throw new NullReferenceException("RepoData is null in RepoRecordToRepoObject converter.");

		x.RepoName = repoData.Name;
		x.DateRetrieved = repoData.DateRetrieved;
		x.Properties = new RepoProperties
		{
			RepoName = repoData.Name,
			DateRetrieved = repoData.DateRetrieved,
			CoveredLines = repoData.CoveredLines,
			NumLines = repoData.TotalLines,
			CoveragePercent = repoData.CoveragePercent,
			UncoveredLines = repoData.UncoveredLines,
			Classes = repoData.ListClasses
		};

		return x;
	}
}