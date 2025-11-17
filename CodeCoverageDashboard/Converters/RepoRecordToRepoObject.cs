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

public static class RepoRecordToRepoObject
{
    //RepositoryRecord Record to RepoData Object Converter
    public static object? Convert(object? value)
    {
        var x = new RepoData();
        var repoRecord = (RepoRecord)value ?? throw new NullReferenceException("RepoRecord is null in RepoRecordToRepoObject converter.");

        x.Name = repoRecord.RepoName;

        var properties = repoRecord.Properties ?? throw new NullReferenceException("RepoRecord.Properties is null in RepoRecordToRepoObject converter.");

        x.DateRetrieved = repoRecord.DateRetrieved;

        x.CoveredLines = properties.CoveredLines;
        x.TotalLines = properties.NumLines;
        x.CoveragePercent = properties.CoveragePercent;
        x.ListClasses = properties.Classes;
        x.CoveragePercentPercentIncrease = properties.CoveragePercentPercentIncrease;
        x.BranchRate = properties.BranchRate;
        x.TotalBranches = properties.TotalBranches;
        x.TotalCoveredBranches = properties.TotalCoveredBranches;
        x.CoveredLinesIncrease = properties.CoveredLinesIncrease;


        return x;
    }

    //RepoData Object to RepositoryRecord Record Converter
    public static object? ConvertBack(object? value)
    {
        var x = new RepoRecord();
        var repoData = (RepoData)value ?? throw new NullReferenceException("RepoData is null in RepoRecordToRepoObject converter.");

        x.RepoName = repoData.Name;
        x.DateRetrieved = repoData.DateRetrieved;
        x.Properties = new RepoProperties
        {
            RepoName = repoData.Name,
            CoveredLines = repoData.CoveredLines,
            NumLines = repoData.TotalLines,
            CoveragePercent = repoData.CoveragePercent,
            UncoveredLines = repoData.UncoveredLines,
            Classes = repoData.ListClasses,
            CoveragePercentPercentIncrease = repoData.CoveragePercentPercentIncrease,
            BranchRate = repoData.BranchRate,
            TotalBranches = repoData.TotalBranches,
            TotalCoveredBranches = repoData.TotalCoveredBranches,
            CoveredLinesIncrease = repoData.CoveredLinesIncrease
        };

        return x;
    }
}
