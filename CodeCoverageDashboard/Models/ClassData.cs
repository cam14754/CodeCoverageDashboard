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

public class ClassData
{
    public string Name { get; set; } = "Unknown Name";
    public double CoveragePercent { get; set; } = 0;
    public double BranchCoveragePercent { get; set; } = 0;
    public double TotalLines { get; set; } = 0;
    public double CoveredLines { get; set; } = 0;

    public DateTime DateRetrieved { get; set; } = DateTime.UnixEpoch;

    public string FilePath { get; set; } = "Unknown FilePath";
    public List<MethodData> ListMethods { get; set; } = [];
}
