// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Text;
using System.Xml.Serialization;

namespace CodeCoverageDashboard.Services;
public class RepoCoverageAnalyzer : IRepoCoverageAnalyzer
{
	public async Task<RepoData> AnalyzeRepoAsync(RepoData repo)
	{
		var watch = Stopwatch.StartNew();

		// Run settings for optimizing code coverage collection
		var runSettingsPath = @"C:\Users\cam14754\Desktop\Unit Testing Intern Project\CodeCoverageDashboard\CodeCoverageDashboard\CodeCoverage.runsettings";

		// Path to the results directory
		string resultsDirectoryPath = Path.Combine(FileSystem.AppDataDirectory, ".coverage");

		// Run the commands

		if (repo.AbsolutePath is null)
		{
			Debug.WriteLine($"Repo URL is null for repo {repo.Name}. Skipping...");
			return null;
		}

		if (RepoIgnore.IgnoreReposList.Contains(repo.AbsolutePath))
		{
			Debug.WriteLine($"Ignoring repo {repo.Name}. Skipping...");
			return null;
		}

		string srcPath = Path.Combine(repo.AbsolutePath, "tests");

		if (!Directory.Exists(srcPath))
		{
			Debug.WriteLine($"Tests directory not found for repo {repo.Name} at path {srcPath}. Skipping...");
			return null;
		}

		string? childDir = Directory.GetDirectories(srcPath).First();

		if (childDir is null)
		{
			Debug.WriteLine($"No child directories found in tests directory for repo {repo.Name} at path {srcPath}. Skipping...");
			return null;
		}

		string? csproj = Directory.GetFiles(childDir, "*.csproj", SearchOption.TopDirectoryOnly).First();

		if (csproj is null)
		{
			Debug.WriteLine($"No .csproj file found in {childDir} for repo {repo.Name}. Skipping...");
			return null;
		}

		Debug.WriteLine($"Found .csproj at {csproj}");

		// Build the dotnet test args
		var sb = new StringBuilder();
		sb.Append("test \"")
		  .Append(csproj)
		  .Append("\" ")
		  .Append("--collect:\"XPlat Code Coverage\" ")
		  .Append("--results-directory \"").Append(resultsDirectoryPath).Append("\" ")
		  .Append("--settings:\"").Append(runSettingsPath).Append("\" ")
		  .Append("--no-restore ")
		  .Append("--nologo ")
		  .Append("--no-build");

		string args = sb.ToString();

		await ExtractData(repo, resultsDirectoryPath, args);

		watch.Stop();
		var elapsedMs = watch.ElapsedMilliseconds;
		Debug.WriteLine($"Analysis completed in {elapsedMs} ms.");

		return repo;
	}

	static async Task ExtractData(RepoData repoData, string resultDirPath, string args)
	{
		Debug.WriteLine($"Running test command on {repoData.Name}");
		await Task.Run(() =>
		{
			var process = Process.Start(new ProcessStartInfo("dotnet", args)
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			});

			process!.WaitForExit();
		});


		// Find the Cobertura coverage file that was generated
		string coverageFilePath = Directory.GetFiles(resultDirPath, "coverage.cobertura.xml", SearchOption.AllDirectories)[0];

		string newFilePath = $"{resultDirPath}\\coverage_{repoData.ID}.cobertura.xml";
		File.Move(coverageFilePath, newFilePath);
		coverageFilePath = newFilePath;

		Debug.WriteLine($"Created Coverage file at: {coverageFilePath}");

		foreach (var dir in Directory.GetDirectories(resultDirPath))
		{
			Directory.Delete(dir, true); // 'true' means recursive delete
		}

		//Load Document
		XmlSerializer xs = new XmlSerializer(typeof(DTOs.CoverageDto));
		using FileStream fs = File.Open(coverageFilePath, FileMode.Open);
		DTOs.CoverageDto coverage = (DTOs.CoverageDto)xs.Deserialize(fs);

		// root attributes
		repoData.CoveredLines = coverage.LinesCovered;
		repoData.TotalLines = coverage.LinesValid;
		repoData.CoveragePercent = coverage.LineRate;
		repoData.UncoveredLines = repoData.TotalLines - repoData.CoveredLines;

		// first (or only) package
		DTOs.PackageDto package = coverage.Packages.FirstOrDefault();

		repoData.ListClasses = package?.Classes
			.Select(c => new ClassData
			{
				Name = c.Name,
				CoveragePercent = c.LineRate,

				Methods = c.Methods.Select(m => new MethodData
				{
					Name = m.Name,
					CoveragePercent = m.LineRate
				})
				.ToList() ?? []
			})
			.ToList() ?? [];

		if (coverage.Packages.Count == 0)
		{
			repoData.Errors.Add("Packages Count is 0, ensure .csproj reference setup properly.");
		}

		Debug.WriteLine("Added data to objects");
	}
}

