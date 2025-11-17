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

public class MethodDTO
{
    [XmlAttribute("name")] public string Name { get; set; } = "";
    [XmlAttribute("line-rate")] public double LineRate { get; set; }
    [XmlAttribute("signature")] public string Signature { get; set; } = "()";
    [XmlAttribute("complexity")] public double Complexity { get; set; }
    [XmlAttribute("branch-rate")] public double BranchRate { get; set; }

    [XmlArray("lines")]
    [XmlArrayItem("line")]
    public List<LineDTO> Lines { get; set; } = [];
}
