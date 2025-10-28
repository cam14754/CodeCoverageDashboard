// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CodeCoverageDashboard.Services;
public class RepoCoverageAnalyzer : IRepoCoverageAnalyzer
{
	public static void AnalyzeRepo(RepoData repoData)
	{
		ArgumentNullException.ThrowIfNull(repoData);
		DTOs.CoverageDto? coverage = null;
		try
		{
			// Deserialize from provided XDocument
			var serializer = new XmlSerializer(typeof(DTOs.CoverageDto));
			using var reader = repoData.XDocument.CreateReader();
			coverage = serializer.Deserialize(reader) as DTOs.CoverageDto;
		}
		catch (Exception ex)
		{
			repoData.ListErrors.Add("Exception during XML deserialization: " + ex.Message);
			Debug.WriteLine("Exception during XML deserialization for repo: " + repoData.Name + " Exception: " + ex.Message);
			return;
		}

		if (coverage is null)
		{
			repoData.ListErrors.Add("Deserialized coverage object is null.");
			Debug.WriteLine("Deserialized coverage object is null for repo: " + repoData.Name);
			return;
		}

		if (!TryParseRepoMetadata(repoData, coverage))
		{
			Debug.WriteLine("Failed to parse repo metadata for repo: " + repoData.Name);
			return;
		}

		if (!TryAnalyzeXDocument(repoData, coverage))
		{
			Debug.WriteLine("Failed to analyze XDocument for repo: " + repoData.Name);
			return;
		}

		Debug.WriteLine("Successfully analyzed repo: " + repoData.Name);
	}
	public static bool TryParseRepoMetadata(RepoData repoData, DTOs.CoverageDto? coverage)
	{
		try
		{
			repoData.Name = coverage!.Packages.First().Name;
			repoData.DateRetrieved = DateTime.Now;
		}
		catch (Exception ex)
		{
			repoData.ListErrors.Add("Exception during parsing repo metadata: " + ex.Message);
			Debug.WriteLine("Exception during parsing repo metadata for repo: " + repoData.Name + " Exception: " + ex.Message);
			return false;
		}

		return true;
	}
	public static bool TryAnalyzeXDocument(RepoData repoData, DTOs.CoverageDto? coverage)
	{
		if (repoData.XDocument is null)
		{
			repoData.ListErrors.Add("Input XDocument is null");
			return false;
		}



		if (coverage is null)
		{
			repoData.ListErrors.Add("Failed to deserialize coverage XML.");
			return false;
		}

		// root attributes
		repoData.CoveredLines = coverage.LinesCovered;
		repoData.TotalLines = coverage.LinesValid;
		repoData.CoveragePercent = coverage.LineRate;
		repoData.UncoveredLines = repoData.TotalLines - repoData.CoveredLines;

		if (coverage.Packages is null)
		{
			repoData.ListErrors.Add("Package is null");
			return false;
		}

		if (coverage.Packages.Count == 0)
		{
			repoData.ListErrors.Add("Packages Count is 0, ensure .csproj reference setup properly.");
			return false;
		}

		DTOs.PackageDto? package = coverage.Packages.FirstOrDefault();

		if (package is null)
		{
			repoData.ListErrors.Add("Package is null");
			return false;
		}

		var classMap = new Dictionary<string, ClassData>();

		// Helper to get or create the parent class
		ClassData GetOrAddClass(string name, double? coverage)
		{
			if (!classMap.TryGetValue(name, out var cd))
			{
				cd = new ClassData
				{
					Name = name,
					CoveragePercent = coverage,
					ListMethods = []
				};
				classMap[name] = cd;
			}
			return cd;
		}

		string AsyncLambdaInsideMethod = "(?:.*?\\.)?(?<parent>[^.]+)\\/<>c__DisplayClass\\d+_\\d+\\/<<(?<method>[^>]+)>b__\\d+>d?$";
		string ClosureClassAsyncLambda = "(?:.*?\\.)?(?<parent>[^.]+)\\/<>c__DisplayClass\\d+_\\d+\\/<<(?<method>[^>]+)>b__\\d+(?:_\\d+)?>d$";
		string IteratorStateMachine = "(?:.*?\\.)?(?<parent>[^.]+)\\/<(?<method>[^>]+)>d__\\d+$";
		string RegularClassName = "(?<=\\.)(?<parent>[^.\\/<+]+)$";
		//string MatchType = "\\.(?<type>[A-Za-z0-9_]+)\\s*(?=[),])";

		foreach (var c in package.Classes)
		{
			Match match;

			// 1) Async lambda inside method
			match = Regex.Match(c.Name, AsyncLambdaInsideMethod);
			if (match.Success)
			{
				//Debug.WriteLine($"Processing class: {c.Name} as an async lambda inside method");
				var className = match.Groups["parent"].Value;
				var methodName = match.Groups["method"].Value + " (Async Lambda)";
				var parent = GetOrAddClass(className, c.LineRate);
				parent.ListMethods.Add(new MethodData
				{
					Name = methodName,
					CoveragePercent = c.LineRate,
					ListLines = c.Lines
				});
				continue;
			}

			// 2) Closure class async lambda
			match = Regex.Match(c.Name, ClosureClassAsyncLambda);
			if (match.Success)
			{
				//Debug.WriteLine($"Processing class {c.Name} as an async lambda");
				var className = match.Groups["parent"].Value;
				var methodName = match.Groups["method"].Value + " (Closure Class Async Lambda)";
				var parent = GetOrAddClass(className, c.LineRate);
				parent.ListMethods.Add(new MethodData
				{
					Name = methodName,
					CoveragePercent = c.LineRate,
					ListLines = c.Lines
				});
				continue;
			}

			// 3) Iterator/async state machine
			match = Regex.Match(c.Name, IteratorStateMachine);
			if (match.Success)
			{
				//Debug.WriteLine($"Processing class {c.Name} as an iterator state machine");
				var className = match.Groups["parent"].Value;
				var methodName = match.Groups["method"].Value + " (Async Method)";
				var parent = GetOrAddClass(className, c.LineRate);
				parent.ListMethods.Add(new MethodData
				{
					Name = methodName,
					CoveragePercent = c.LineRate,
					ListLines = c.Lines
				});
				continue;
			}

			// 4) Regular class with real methods
			match = Regex.Match(c.Name, RegularClassName);
			if (match.Success)
			{
				//Debug.WriteLine($"Processing class: {c.Name} as a regular method");
				var className = match.Groups["parent"].Value;
				var parent = GetOrAddClass(className, c.LineRate);

				foreach (var m in c.Methods)
				{
					var lines = new List<LineData>();
					foreach (var l in m.Lines)
					{
						lines.Add(new LineData { LineNumber = l.Number, Hits = l.Hits });
					}

					parent.ListMethods.Add(new MethodData
					{
						Name = m.Name,
						CoveragePercent = m.LineRate,
						ListLines = lines,
						Signature = m.Signature

					});
				}
			}
			// else: discard other synthesized noise
		}

		repoData.ListClasses = [.. classMap.Values];

		if (repoData.ListClasses.Count == 0)
		{
			repoData.ListErrors.Add("Classes Count is 0");
		}

		Debug.WriteLine("Successfully added data to objects");
		return true;
	}
}

