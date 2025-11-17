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

[XmlRoot("package")]
public class PackageDTO
{
    [XmlAttribute("name")] public string Name { get; set; } = string.Empty;
    [XmlAttribute("line-rate")] public double LineRate { get; set; } = 0;
    [XmlAttribute("branch-rate")] public double BranchRate { get; set; } = 0;
    [XmlAttribute("complexity")] public double Complexity { get; set; } = 0;

    [XmlArray("classes")]
    [XmlArrayItem("class")]
    public List<ClassDTO> Classes { get; set; } = [];
}
