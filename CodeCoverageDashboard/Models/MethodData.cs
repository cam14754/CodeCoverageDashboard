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

public class MethodData
{
    public string? Name { get; set; } = "Unknown Name";
    public string[]? Errors { get; set; } = [];
    public double CoveragePercent { get; set; } = 0;
    public double BranchCoveragePercent { get; set; } = 0;

    public double Complexity { get; set; } = 0;
    public List<LineData> ListLines { get; set; } = [];
    public string? Signature { get; set; } = "()";
}
