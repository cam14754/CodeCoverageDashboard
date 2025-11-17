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

namespace CodeCoverageDashboard.DTOs;

[XmlRoot("coverage")]
public class CoverageDTO
{
    [XmlAttribute("line-rate")] public double LineRate { get; set; }
    [XmlAttribute("branch-rate")] public double BranchRate { get; set; }
    [XmlAttribute("version")] public string Version { get; set; } = string.Empty;
    [XmlAttribute("timestamp")] public long Timestamp { get; set; }
    [XmlAttribute("lines-covered")] public double LinesCovered { get; set; }
    [XmlAttribute("lines-valid")] public double LinesValid { get; set; }
    [XmlAttribute("branches-covered")] public double BranchesCovered { get; set; }
    [XmlAttribute("branches-valid")] public double BranchesValid { get; set; }

    [XmlArray("packages")]
    [XmlArrayItem("package")]
    public List<PackageDTO> Packages { get; set; } = [];

    [XmlArray("sources")]
    [XmlArrayItem("source")]
    public List<string> Sources { get; set; } = [];
}
