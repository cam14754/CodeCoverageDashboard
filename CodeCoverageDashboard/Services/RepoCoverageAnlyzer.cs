// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Services;

public class RepoCoverageAnalyzer : IRepoCoverageAnalyzer
{
	public static void AnalyzeRepo(RepoData repoData)
	{
		ArgumentNullException.ThrowIfNull(repoData);

		try
		{
			// Deserialize from provided XDocument
			var serializer = new XmlSerializer(typeof(CoverageDTO));
			using var reader = repoData.XDocument.CreateReader();
			CoverageDTO coverage = serializer.Deserialize(reader) as CoverageDTO;

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
		}
		catch (Exception ex)
		{
			repoData.ListErrors.Add("Exception during XML deserialization: " + ex.Message);
			Debug.WriteLine("Exception during XML deserialization for repo: " + repoData.Name + " Exception: " + ex.Message);
			return;
		}

	}
	public static bool TryParseRepoMetadata(RepoData repoData, CoverageDTO? coverage)
	{
		try
		{
			repoData.Name = coverage!.Packages.First().Name;
			repoData.DateRetrieved = DateTime.UnixEpoch.AddSeconds(coverage.Timestamp).ToLocalTime();
		}
		catch (Exception ex)
		{
			repoData.ListErrors.Add("Exception during parsing repo metadata: " + ex.Message);
			Debug.WriteLine("Exception during parsing repo metadata for repo: " + repoData.Name + " Exception: " + ex.Message);
			return false;
		}

		return true;
	}
	public static bool TryAnalyzeXDocument(RepoData repoData, CoverageDTO? coverage)
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
		repoData.BranchRate = coverage.BranchRate;
		repoData.TotalBranches = coverage.BranchesValid;
		repoData.TotalCoveredBranches = coverage.BranchesCovered;
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

		PackageDTO package = coverage.Packages.FirstOrDefault();

		if (package is null)
		{
			repoData.ListErrors.Add("Package is null");
			return false;
		}

		var ListClasses = new List<ClassData>();
		foreach (var c in package.Classes)
		{
			var ListMethods = new List<MethodData>();
			foreach (var m in c.Methods)
			{
				var lines = new List<LineData>();
				foreach (var l in m.Lines)
				{
					lines.Add(new LineData { LineNumber = l.Number, Hits = l.Hits });
				}

				ListMethods.Add(new MethodData
				{
					Name = m.Name,
					CoveragePercent = m.LineRate,
					ListLines = lines,
					Signature = m.Signature,
					Complexity = m.Complexity,
				});
			}

			ListClasses.Add(new ClassData
			{
				Name = c.Name,
				FilePath = c.FileName,
				CoveragePercent = c.LineRate,
				ListMethods = ListMethods,
			});
		}
	

		repoData.ListClasses = ListClasses;

		if (repoData.ListClasses.Count == 0)
		{
			repoData.ListErrors.Add("Classes Count is 0");
		}

		return true;
	}
}