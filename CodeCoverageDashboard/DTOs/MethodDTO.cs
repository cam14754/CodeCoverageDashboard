// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

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