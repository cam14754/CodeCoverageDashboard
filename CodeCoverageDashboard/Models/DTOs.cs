// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.


using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CodeCoverageDashboard.Models;
public class DTOs
{
	[XmlRoot("coverage")]
	public class CoverageDto
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
		public List<PackageDto> Packages { get; set; } = [];

		[XmlArray("sources")]
		[XmlArrayItem("source")]
		public List<string> Sources { get; set; } = [];
	}

	public class SourceDto
	{
		[XmlText]
		public string Path { get; set; } = "";
	}

	[XmlRoot("package")]
	public class PackageDto
	{
		[XmlAttribute("name")] public string Name { get; set; } = String.Empty;
		[XmlAttribute("line-rate")] public double LineRate { get; set; } = 0;
		[XmlAttribute("branch-rate")] public double BranchRate { get; set; } = 0;
		[XmlAttribute("complexity")] public double Complexity { get; set; } = 0;

		[XmlArray("classes")]
		[XmlArrayItem("class")]
		public List<ClassDto> Classes { get; set; } = [];
	}

	public class ClassDto
	{
		[XmlAttribute("name")] public string Name { get; set; } = String.Empty;
		[XmlAttribute("filename")] public string FileName { get; set; } = String.Empty;
		[XmlAttribute("line-rate")] public double LineRate { get; set; } = 0;
		[XmlAttribute("branch-rate")] public double BranchRate { get; set; } = 0;
		[XmlAttribute("complexity")] public double Complexity { get; set; } = 0;

		[XmlArray("methods")]
		[XmlArrayItem("method")]
		public List<MethodDto> Methods { get; set; } = [];

		[XmlArray("lines")]
		[XmlArrayItem("line")]
		public List<LineData> Lines { get; set; } = [];
	}

	public class MethodDto
	{
		[XmlAttribute("name")] public string Name { get; set; } = "";
		[XmlAttribute("line-rate")] public double LineRate { get; set; }
		[XmlAttribute("signature")] public string Signature { get; set; } = "()";
		[XmlAttribute("complexity")] public double Complexity { get; set; }
		[XmlAttribute("branch-rate")] public double BranchRate { get; set; }

		[XmlArray("lines")]
		[XmlArrayItem("line")]
		public List<LineDto> Lines { get; set; } = [];
	}

	public class LineDto
	{
		[XmlAttribute("number")] public double Number { get; set; }
		[XmlAttribute("hits")] public double Hits { get; set; }
		[XmlAttribute("branch")] public string Branch { get; set; } = "false";
		[XmlAttribute("condition-coverage")] public string ConditionCoverage { get; set; } = String.Empty;
	}

	public class RepoProperties
	{
		[JsonPropertyName("RepoName")]
		public string RepoName { get; set; } = string.Empty;

		[JsonPropertyName("DateRetrieved")]
		public DateTime DateRetrieved { get; set; } = DateTime.MinValue;

		[JsonPropertyName("CoveredLines")]
		public double CoveredLines { get; set; } = 0;

		[JsonPropertyName("NumLines")]
		public double NumLines { get; set; } = 0;

		[JsonPropertyName("CoveragePercent")]
		public double CoveragePercent { get; set; } = 0;

		[JsonPropertyName("UncoveredLines")]
		public double UncoveredLines { get; set; } = 0;
	}

	public class DashboardProperties
	{
		[JsonPropertyName("DateRetrieved")]
		public DateTime DateRetrieved { get; set; } = DateTime.MinValue;

		[JsonPropertyName("TotalCoveredLines")]
		public double TotalCoveredLines { get; set; } = 0;

		[JsonPropertyName("AverageCoveragePercent")]
		public double AverageCoveragePercent { get; set; } = 0;

		[JsonPropertyName("TotalNumLines")]
		public double TotalNumLines { get; set; } = 0;

		[JsonPropertyName("TotalUncoveredLines")]
		public double TotalUncoveredLines { get; set; } = 0;
	}

	public class MethodProperties
	{
		[JsonPropertyName("ClassName")]
		public string ClassName { get; set; } = string.Empty;

		[JsonPropertyName("MethodName")]
		public string MethodName { get; set; } = string.Empty;

		[JsonPropertyName("Signature")]
		public string Signature { get; set; } = string.Empty;

		[JsonPropertyName("CoveragePercent")]
		public double CoveragePercent { get; set; } = 0;

		[JsonPropertyName("Lines")]
		public List<LineData> Lines { get; set; } = [];
	}
}
