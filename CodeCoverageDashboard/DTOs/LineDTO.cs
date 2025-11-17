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

public class LineDTO
{
    [XmlAttribute("number")] public double Number { get; set; }
    [XmlAttribute("hits")] public double Hits { get; set; }
    [XmlAttribute("branch")] public string Branch { get; set; } = "false";
    [XmlAttribute("condition-coverage")] public string ConditionCoverage { get; set; } = string.Empty;
}
