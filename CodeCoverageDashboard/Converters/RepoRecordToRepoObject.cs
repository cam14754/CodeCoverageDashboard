// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Converters;

public static class DashboardRecordToDashboardObject
{
	public static object? Convert(object? value)
	{
		value = (DashboardRecord)value;

		var staticDashBoardObject = new StaticDashboardData();
		{
			staticDashBoardObject.TotalLines = value.Proper.TotalLines;
			staticDashBoardObject.TotalCoveredLines = value.Properties.CoveredLines;
			staticDashBoardObject.AverageCoverage = value.Properties.LineCoverage;
			staticDashBoardObject.AverageBranchCoverage = value.Properties.BranchCoverage;
			staticDashBoardObject.TotalRepos = value.Properties.TotalRepos;
			staticDashBoardObject.TotalClasses = value.Properties.TotalClasses;
			staticDashBoardObject.TotalMethods = value.Properties.TotalMethods;
			staticDashBoardObject.DateRetrieved = value.Properties.LastUpdated;
		}
		;

		return staticDashBoardObject;
	}

	public static object? ConvertBack(object? value)
	{

	}
}