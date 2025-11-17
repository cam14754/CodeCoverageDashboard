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

namespace CodeCoverageDashboard.Models;

public partial class StaticDashboardData : ObservableObject
{
    //Calculated properties
    [NotifyPropertyChangedFor(nameof(TotalLinesUncoveredCount))]
    [ObservableProperty] public partial double TotalLinesCoveredCount { get; set; } = 0; //
    [ObservableProperty] public partial double TotalReposCount { get; set; } = 0; //
    [ObservableProperty] public partial double TotalClassesCount { get; set; } = 0; //
    [NotifyPropertyChangedFor(nameof(AverageComplexMethodPercent))]
    [NotifyPropertyChangedFor(nameof(TotalComplexMethodCount))]

    [ObservableProperty] public partial double TotalMethodsCount { get; set; } = 0; //
    [ObservableProperty] public partial double TotalLinesCount { get; set; } = 0; //
    [ObservableProperty] public partial double AverageBranchCoveragePercent { get; set; } = 0; //
    [ObservableProperty] public partial double AverageLineCoveragePercent { get; set; } = 0; //
    [ObservableProperty] public partial double TotalBracnhesCoveredCount { get; set; } = 0; //



    //Maybe Temp?
    public ObservableCollection<RepoData> ListRepos { get; set; } = []; //
    public ObservableCollection<MethodData> ListMethods { get; set; } = []; //


    //Given properties
    [ObservableProperty] public partial DateTime DateRetrieved { get; set; } = DateTime.UnixEpoch; //
    [ObservableProperty] public partial DateTime DataAge { get; set; } = DateTime.UnixEpoch; //
    [ObservableProperty] public partial string CoverletVersion { get; set; } = string.Empty; //
    [ObservableProperty] public partial string DashboardVersion { get; set; } = string.Empty; //

    // Calculated inferred properties
    //public int TotalRepos => ListRepos.Count;
    public double TotalLinesUncoveredCount => TotalLinesCount - TotalLinesCoveredCount; //	
    public double AverageComplexMethodPercent => TotalComplexMethodCount / ListMethods.Count; //
    public double TotalComplexMethodCount => ListMethods.Where(m => m.Complexity >= 10).Count(); //
}
