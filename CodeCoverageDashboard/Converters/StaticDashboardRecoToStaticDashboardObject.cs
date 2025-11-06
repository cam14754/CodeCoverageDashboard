// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Converters;

public static class StaticDashboardRecordToStaticDashboardObject
{
	// Convert DashboardRecord -> StaticDashboardData (observable object)
	public static object? Convert(object? value)
	{
		if (value is null)
		{
			return null;
		}

		if (value is not DashboardRecord dashboard)
		{
			return null;
		}

		if(dashboard.Properties is null)
		{
			return null;
		}

		var model = new StaticDashboardData
		{
			// Properties
			TotalLinesCoveredCount = dashboard.Properties.TotalLinesCoveredCount,
			TotalReposCount = dashboard.Properties.TotalReposCount,
			TotalClassesCount = dashboard.Properties.TotalClassesCount,
			TotalMethodsCount = dashboard.Properties.TotalMethodsCount,
			TotalLinesCount = dashboard.Properties.TotalLinesCount,
			AverageLineCoveragePercent = dashboard.Properties.AverageLineCoveragePercent,
			AverageBranchCoveragePercent = dashboard.Properties.AverageBranchCoveragePercent,
			TotalBracnhesCoveredCount = dashboard.Properties.TotalBracnhesCoveredCount,
			DateRetrieved = dashboard.Properties.DateRetrieved,
			CoverletVersion = dashboard.Properties.CoverletVersion,
			DashboardVersion = dashboard.Properties.DashboardVersion,
			DataAge = dashboard.Properties.DataAge,
			// Collections
			ListMethods = new ObservableCollection<MethodData>(dashboard.Properties.ComplexMethods),
			ListRepos = new ObservableCollection<RepoData>(dashboard.Properties.ListRepos),
		};
		return model;
	}

	// Convert StaticDashboardData -> DashboardRecord
	public static object? ConvertBack(object? value)
	{
		if (value is null)
		{
			return null;
		}

		if (value is not StaticDashboardData data)
		{
			return null;
		}

		var props = new DashboardProperties
		{
			TotalLinesCoveredCount = data.TotalLinesCoveredCount,
			TotalReposCount = data.TotalReposCount,
			TotalClassesCount = data.TotalClassesCount,
			TotalMethodsCount = data.TotalMethodsCount,
			TotalLinesCount = data.TotalLinesCount,
			AverageLineCoveragePercent = data.AverageLineCoveragePercent,
			AverageBranchCoveragePercent = data.AverageBranchCoveragePercent,
			TotalBracnhesCoveredCount = data.TotalBracnhesCoveredCount,
			AverageComplexMethodPercent = data.AverageComplexMethodPercent,
			DateRetrieved = data.DateRetrieved,
			CoverletVersion = data.CoverletVersion,
			DashboardVersion = data.DashboardVersion,
			ComplexMethods = new ObservableCollection<MethodData>(data.ListMethods),
			ListRepos = new ObservableCollection<RepoData>(data.ListRepos),
			TotalLinesUncoveredCount = data.TotalLinesUncoveredCount,
			DataAge = data.DataAge
		};

		var record = new DashboardRecord
		{
			DateRetrieved = data.DateRetrieved,
			Properties = props
		};

		return record;
	}
}