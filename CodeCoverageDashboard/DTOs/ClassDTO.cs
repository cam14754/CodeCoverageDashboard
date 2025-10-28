// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.DTOs;
public class ClassDTO
{
	[XmlAttribute("name")] public string Name { get; set; } = string.Empty;
	[XmlAttribute("filename")] public string FileName { get; set; } = string.Empty;
	[XmlAttribute("line-rate")] public double LineRate { get; set; } = 0;
	[XmlAttribute("branch-rate")] public double BranchRate { get; set; } = 0;
	[XmlAttribute("complexity")] public double Complexity { get; set; } = 0;

	[XmlArray("methods")]
	[XmlArrayItem("method")]
	public List<MethodDTO> Methods { get; set; } = [];

	[XmlArray("lines")]
	[XmlArrayItem("line")]
	public List<LineDTO> Lines { get; set; } = [];
}