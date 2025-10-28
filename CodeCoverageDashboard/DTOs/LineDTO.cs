// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.DTOs;
public class LineDTO
{
	[XmlAttribute("number")] public double Number { get; set; }
	[XmlAttribute("hits")] public double Hits { get; set; }
	[XmlAttribute("branch")] public string Branch { get; set; } = "false";
	[XmlAttribute("condition-coverage")] public string ConditionCoverage { get; set; } = string.Empty;
}