// COPYRIGHT © 2025 ESRI
//
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States.
//
// For additional information, contact:
// Environmental Systems Research Institute, Inc.
// Attn: Contracts Dept
// 380 New York Street
// Redlands, California, USA 92373
//
// email: contracts@esri.com

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

        if (dashboard.Properties is null)
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
            CoverletVersion = dashboard.Properties.CoverletVersion,
            DashboardVersion = dashboard.Properties.DashboardVersion,
            DataAge = dashboard.Properties.DataAge,
            AverageComplexMethodPercent = dashboard.Properties.AverageComplexMethodPercent,
            TotalLinesUncoveredCount = dashboard.Properties.TotalLinesUncoveredCount,
            TotalComplexMethodCount = dashboard.Properties.TotalComplexMethodCount,
            // Collections
            ListRepos = dashboard.Properties.ListRepos,
            ComplexMethods = dashboard.Properties.ComplexMethods,
            HealthyRepos = dashboard.Properties.HealthyRepos,
            UnhealthyRepos = dashboard.Properties.UnhealthyRepos,
            HotRepos = dashboard.Properties.HotRepos
        };
        return model;
    }

    // Convert StaticDashboardData -> DashboardRecord
    public static object? ConvertBack(object? value)
    {
        Debug.WriteLine("Read-only, no converting to record");
        return null;
    }
}
