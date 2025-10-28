// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

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