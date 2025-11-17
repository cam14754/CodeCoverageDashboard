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

using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace CodeCoverageDashboard.Models;

public partial class RepoData : ObservableObject
{
    [ObservableProperty]
    public partial string AbsolutePath { get; set; } = "Unknown Path";

    [ObservableProperty]
    public partial string Name { get; set; } = "Unknown Name";

    [ObservableProperty]
    public partial ObservableCollection<string> ListErrors { get; set; } = [];

    [ObservableProperty]
    public partial double CoveragePercent { get; set; } = 0;
    [ObservableProperty]
    public partial double CoveragePercentPercentIncrease { get; set; } = 0;

    [ObservableProperty]
    public partial double CoveredLines { get; set; } = 0;
    [ObservableProperty]
    public partial double CoveredLinesIncrease { get; set; } = 0;

    [ObservableProperty]
    public partial double TotalLines { get; set; } = 0;

    [ObservableProperty]
    public partial double UncoveredLines { get; set; } = 0;
    [ObservableProperty]
    public partial double TotalBranches { get; set; } = 0;
    [ObservableProperty]
    public partial double TotalCoveredBranches { get; set; } = 0;
    [ObservableProperty]
    public partial double BranchRate { get; set; } = 0;

    [ObservableProperty]
    public partial DateTime DateRetrieved { get; set; } = DateTime.Now;

    [ObservableProperty]
    public partial List<ClassData> ListClasses { get; set; } = [];

    [ObservableProperty]
    public partial XDocument? XDocument { get; set; } = null;

    //Not to be saved
    [ObservableProperty]
    public partial bool IsHovered { get; set; } = false;

    [RelayCommand]
    public void ToggleHovered()
    {
        IsHovered = !IsHovered;
    }
}
