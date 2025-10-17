// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Xml.Linq;

namespace CodeCoverageDashboard.Services;
public class RepoCoverageAnalyzer : IRepoCoverageAnalyzer
{
	public async Task AnalyzeRepoAsync()
	{
		var watch = Stopwatch.StartNew();

		// Path to the test project
		string testProjectPath = @"C:\Users\cam14754\Desktop\FirstProjects\CalculatorExample\CalculatorExampleTests\CalculatorExampleTests.csproj";
		var newRepo = new RepoData() { Name = "CalculatorExample", Url = testProjectPath, ID = Guid.NewGuid() };
		var runSettingsPath = @"C:\Users\cam14754\Desktop\Unit Testing Intern Project\CodeCoverageDashboard\CodeCoverageDashboard\CodeCoverage.runsettings";

		if (!File.Exists(runSettingsPath))
		{
			Debug.WriteLine($"Runsettings not found at: {runSettingsPath}");
			// bail early or create a temp runsettings file
		}

		// Path to the results directory
		if (!Directory.Exists(Path.Combine(FileSystem.AppDataDirectory, ".coverage")))
		{
			Directory.CreateDirectory($"{Path.Combine(FileSystem.AppDataDirectory, ".coverage")}");
		}
		string resultDirPath = Path.Combine(FileSystem.AppDataDirectory, ".coverage");

		// Arguments to run the test
		string args = $"test \"{testProjectPath}\" " +
			$"--collect:\"XPlat Code Coverage\" " +
			$"--results-directory \"{resultDirPath}\" " +
			$"--settings:\"{runSettingsPath}\" " +
			$"--nologo " +
			$"--no-restore " +
			$"--no-build";


		// Run the command
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

			// Find the Cobertura coverage file that was generated
			string coverageFile = Directory.GetFiles(resultDirPath, "coverage.cobertura.xml", SearchOption.AllDirectories)[0];

			string newFilePath = $"{resultDirPath}\\coverage_{newRepo.ID}.cobertura.xml";
			File.Move(coverageFile, newFilePath);
			coverageFile = newFilePath;

			foreach (var dir in Directory.GetDirectories(resultDirPath))
			{
				Directory.Delete(dir, true); // 'true' means recursive delete
			}

			Debug.WriteLine($"Generated coverage file: {coverageFile}");

			// Parse XML and extract the line-rate attribute from <coverage>
			var doc = XDocument.Load(coverageFile);
			var coverageElement = doc.Root;
			var lineRate = double.Parse(coverageElement!.Attribute("line-rate")!.Value);

			// Convert to percent and print
			double percent = lineRate * 100;
			Debug.WriteLine($"Coverage: {percent:F2}%");


		});
		watch.Stop();
		var elapsedMs = watch.ElapsedMilliseconds;
		Debug.WriteLine($"Analysis completed in {elapsedMs} ms.");
	}
}
