// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.


using System.Xml.Serialization;

namespace CodeCoverageDashboard.Models;
public class DTOs
{
	[XmlRoot("coverage")]
	public class CoverageDto
	{
		[XmlAttribute("line-rate")]
		public double LineRate { get; set; }

		[XmlAttribute("lines-covered")]
		public double LinesCovered { get; set; }

		[XmlAttribute("lines-valid")]
		public double LinesValid { get; set; }

		[XmlArray("packages")]
		[XmlArrayItem("package")]
		public List<PackageDto> Packages { get; set; } = [];
	}

	[XmlRoot("package")]
	public class PackageDto
	{
		[XmlAttribute("name")] public string Name { get; set; }
		[XmlAttribute("line-rate")] public double LineRate { get; set; }

		[XmlArray("classes")]
		[XmlArrayItem("class")]
		public List<ClassDto> Classes { get; set; } = [];
	}

	public class ClassDto
	{
		[XmlAttribute("name")] public string Name { get; set; }
		[XmlAttribute("filename")] public string Filename { get; set; }

		[XmlAttribute("line-rate")] public double LineRate { get; set; }

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

		[XmlArray("lines")]
		[XmlArrayItem("line")]
		public List<LineDto> Lines { get; set; } = [];
	}

	public class LineDto
	{
		[XmlAttribute("number")] public double Number { get; set; }
		[XmlAttribute("hits")] public double Hits { get; set; }
	}



}
