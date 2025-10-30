// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CodeCoverageDashboard.Services;
public class RepoCoverageAnalyzer : IRepoCoverageAnalyzer
{
	public async Task<RepoData> AnalyzeRepoAsync(RepoData repo)
	{
		var watch = Stopwatch.StartNew();

		// Run settings for optimizing code coverage collection
		var runSettingsPath = "C:\\Users\\cam14754\\Desktop\\UnitTestingInternProject\\CodeCoverageDashboard\\CodeCoverageDashboard\\.runsettings";

		// Path to the results directory
		string resultsDirectoryPath = "C:\\Users\\cam14754\\Desktop\\Reports";

		// Run the commands

		if (repo.AbsolutePath is null)
		{
			repo.Errors.Add("The path for this repo is null");
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
			repo.Errors.Add("Test directory was not found!");
			return null;
		}

		string? childDir = Directory.GetDirectories(srcPath).First();

		if (childDir is null)
		{
			Debug.WriteLine($"No child directories found in tests directory for repo {repo.Name} at path {srcPath}. Skipping...");
			repo.Errors.Add("Test child directory was not found!");
			return null;
		}

		string? csproj = Directory.GetFiles(childDir, "*.csproj", SearchOption.TopDirectoryOnly).First();

		if (csproj is null)
		{
			Debug.WriteLine($"No .csproj file found in {childDir} for repo {repo.Name}. Skipping...");
			repo.Errors.Add("No .csproj file found in this directory");
			return null;
		}

		Debug.WriteLine($"Found .csproj at {csproj}");

		// Build the dotnet test args
		var sb = new StringBuilder();
		sb.Append("test \"")
		  .Append(csproj)
		  .Append("\" ")
		  .Append("--collect:\"XPlat Code Coverage\" ")
		  .Append("--settings:\"").Append(runSettingsPath).Append("\" ");

		string args = sb.ToString();

		await ExtractData(repo, resultsDirectoryPath, args);

		watch.Stop();
		var elapsedMs = watch.ElapsedMilliseconds;
		Debug.WriteLine($"Analysis completed in {elapsedMs} ms. \n");

		return repo;
	}

	static async Task ExtractData(RepoData repoData, string resultDirPath, string args)
	{
		Debug.WriteLine($"Running test command on {repoData.Name}");

		await DotnetRunTestAsync(args, repoData);

		// Find the Cobertura coverage file that was generated
		string coverageFilePath = Directory.GetFiles(resultDirPath, "coverage.cobertura.xml", SearchOption.AllDirectories).FirstOrDefault();

		if(coverageFilePath is null)
		{
			repoData.Errors.Add("Coverage file not found");
			Debug.WriteLine($"Coverage file not found for repo {repoData.Name}. Skipping...");
			return;
		}

		string newFilePath = $"{resultDirPath}\\{repoData.Name}_CoverageReport.cobertura.xml";
		File.Move(coverageFilePath, newFilePath);
		coverageFilePath = newFilePath;

		Debug.WriteLine($"Created Coverage file at: {coverageFilePath}");

		foreach (var dir in Directory.GetDirectories(resultDirPath))
		{
			Directory.Delete(dir, true); // 'true' means recursive delete
		}

		//Load Document
		XmlSerializer xs = new(typeof(DTOs.CoverageDto));
		using FileStream fs = File.Open(coverageFilePath, FileMode.Open);
		DTOs.CoverageDto coverage = (DTOs.CoverageDto)xs.Deserialize(fs);

		// root attributes
		repoData.CoveredLines = coverage.LinesCovered;
		repoData.TotalLines = coverage.LinesValid;
		repoData.CoveragePercent = coverage.LineRate;
		repoData.UncoveredLines = repoData.TotalLines - repoData.CoveredLines;

		if (coverage.Packages is null)
		{
			repoData.Errors.Add("Package is null");
			return;
		}

		if (coverage.Packages.Count == 0)
		{
			repoData.Errors.Add("Packages Count is 0, ensure .csproj reference setup properly.");
			return;
		}

		DTOs.PackageDto? package = coverage.Packages.FirstOrDefault();

		if (package is null)
		{
			repoData.Errors.Add("Package is null");
			return;
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
		string RegularClassName = "(?<=\\.)(?<parent>[^.\\/<+]+)$"; // added + to be safe for nested types

		foreach (var c in package.Classes)
		{
			Match match;

			// 1) Async lambda inside method
			match = Regex.Match(c.Name, AsyncLambdaInsideMethod);
			if (match.Success)
			{
				Debug.WriteLine($"Processing class: {c.Name} as an async lambda inside method");
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
				Debug.WriteLine($"Processing class {c.Name} as an async lambda");
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
				Debug.WriteLine($"Processing class {c.Name} as an iterator state machine");
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
				Debug.WriteLine($"Processing class: {c.Name} as a regular method");
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
			repoData.Errors.Add("Classes Count is 0");
		}

		Debug.WriteLine("Successfully added data to objects");
	}

	public static async Task DotnetRunTestAsync(string args, RepoData repoData)
	{
		var startInfo = new ProcessStartInfo
		{
			FileName = "dotnet",
			Arguments = args,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

		if (!process.Start())
		{
			throw new InvalidOperationException("Failed to start process.");
		}

		var stdOutTask = process.StandardOutput.ReadToEndAsync();
		var stdErrTask = process.StandardError.ReadToEndAsync();

		await process.WaitForExitAsync();

		var stdout = await stdOutTask;
		var stderr = await stdErrTask;

		if (process.ExitCode != 0)
		{
			var msg = $"dotnet test failed for {repoData.Name} (exit code:{process.ExitCode})";
			var errormsg = $"Standard Ouput: {stdout} Error Output: {stderr}";
			Debug.WriteLine(msg + errormsg);
			repoData.Errors.Add(msg);
			throw new Exception(msg);
		}
	}
}

