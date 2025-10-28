// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Xml.Linq;

namespace CodeCoverageDashboard.Services;
class HTTPService : IHTTPService
{
	public static async Task<List<XDocument>> GetXDocs()
	{
		//List<RepoData> repoDatas = GetRepoDataAsync();
		//foreach (RepoData repodata in repoDatas)
		//{
		//	await AnalyzeRepoAsync(repodata);
		//}

		//return [.. repoDatas.Select(x => x.XDocument)];
		return await getfromdesk();
	}

	public static async Task<List<XDocument>> getfromdesk()
	{
		return await Task.Run(() =>
		{
			return Directory.GetFiles("C:\\Users\\cam14754\\Desktop\\Reports", "*.cobertura.xml").Select(XDocument.Load).ToList();
		});
	}

	//All of the below is a simulation, as this will effectively be done externally, the DatabaseService for now will return XDocuments. 
	static async Task AnalyzeRepoAsync(RepoData repo)
	{
		var watch = Stopwatch.StartNew();

		if (repo.AbsolutePath is null)
		{
			repo.ListErrors.Add("The path for this repo is null");
			Debug.WriteLine($"Repo URL is null for repo {repo.Name}. Skipping...");
			return;
		}

		if (RepoIgnore.IgnoreReposList.Contains(repo.AbsolutePath))
		{
			Debug.WriteLine($"Ignoring repo {repo.Name}. Skipping...");
			return;
		}

		string srcPath = Path.Combine(repo.AbsolutePath, "tests");

		if (!Directory.Exists(srcPath))
		{
			Debug.WriteLine($"Tests directory not found for repo {repo.Name} at path {srcPath}. Skipping...");
			repo.ListErrors.Add("Test directory was not found!");
			return;
		}

		string? childDir = Directory.GetDirectories(srcPath).First();


		if (childDir is null)
		{
			Debug.WriteLine($"No child directories found in tests directory for repo {repo.Name} at path {srcPath}. Skipping...");
			repo.ListErrors.Add("Test child directory was not found!");
			return;
		}

		string? csproj = Directory.GetFiles(childDir, "*.csproj", SearchOption.TopDirectoryOnly).First();
		string? dllFile = Directory.GetFiles(childDir, "*.UnitTests.dll", SearchOption.AllDirectories).FirstOrDefault();
		string runSettingsPath = @"C:\Users\cam14754\Desktop\UnitTestingInternProject\CodeCoverageDashboard\CodeCoverageDashboard\CodeCoverage.runsettings";
		string resultsDirectoryPath = Path.Combine(FileSystem.AppDataDirectory, $"coverage");
		string testAdapterPath = @"C:\Users\cam14754\.nuget\packages\coverlet.collector\6.0.4\build\netstandard2.0";

		if (csproj is null)
		{
			Debug.WriteLine($"No .csproj file found in {childDir} for repo {repo.Name}. Skipping...");
			repo.ListErrors.Add("No .csproj file found in this directory");
			return;
		}

		if (dllFile is null)
		{
			Debug.WriteLine($"No UnitTests.dll file found in {childDir} for repo {repo.Name}. Skipping...");
			repo.ListErrors.Add("No UnitTests.dll file found in this directory, make sure to run tk test");
			return;
		}

		Debug.WriteLine($"Found .csproj at {csproj}");
		Debug.WriteLine($"Found .dll at {dllFile}");

		// Build the dotnet test args
		string args = $"""test "{dllFile}" --nologo --settings "{runSettingsPath}" --results-directory "{resultsDirectoryPath}" --collect:"XPlat Code Coverage" --test-adapter-path "{testAdapterPath}" """;

		Debug.WriteLine($"Running test command on {repo.Name}");
		await DotnetRunTestAsync(args, repo);

		// Debug.WriteLine($"Moving coverage report file for {repo.Name}");
		MoveCoverageReport(repo, resultsDirectoryPath);

		watch.Stop();
		var elapsedMs = watch.ElapsedMilliseconds;
		Debug.WriteLine($"Analysis completed in {elapsedMs} ms. \n");
	}

	static void MoveCoverageReport(RepoData repo, string resultDirPath)
	{
		// Find the Cobertura coverage file that was generated
		string? coverageFilePath = Directory.GetFiles(resultDirPath, "coverage.cobertura.xml", SearchOption.AllDirectories).FirstOrDefault();

		if (coverageFilePath is null)
		{
			Debug.WriteLine($"Coverage file not found for repo {repo.Name} in results directory {resultDirPath}.");
			repo.ListErrors.Add("Coverage file was not found after running tests.");
			return;
		}

		File.Copy(coverageFilePath, $"C:\\Users\\cam14754\\Desktop\\Reports\\{repo.Name}-CoverageReport.cobertura.xml");

		repo.XDocument = XDocument.Load(coverageFilePath);

		foreach (var dir in Directory.GetDirectories(resultDirPath))
		{
			Directory.Delete(dir, true);
		}
	}

	static async Task DotnetRunTestAsync(string args, RepoData repoData)
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
			repoData.ListErrors.Add(msg);
			throw new Exception(msg);
		}
	}

	public static List<RepoData> GetRepoDataAsync()
	{
		string? repoDirectory = Environment.GetEnvironmentVariable("REPODIR") ?? throw new Exception("Environment variable 'REPODIR' is not set.");
		List<string> repoDirectories = [.. Directory.GetDirectories(repoDirectory)];

		var list = new List<RepoData>();

		foreach (var raw in repoDirectories)
		{
			if (RepoIgnore.IgnoreReposList.Contains(raw))
			{
				Debug.WriteLine($"Ignoring repo {raw}");
				continue;
			}

			if (raw is null)
			{
				throw new ArgumentNullException($"Null Exception: {raw}");
			}
			var data = ParseRepoUrl(raw);
			list.Add(data);


			Debug.WriteLine($"Parsed: \nURL: {data.AbsolutePath} \nName: {data.Name} \nDate Retrieved: {data.DateRetrieved}");

		}

		return list;
	}

	static RepoData ParseRepoUrl(string url)
	{
		try
		{
			Uri.TryCreate(url, UriKind.Absolute, out var uri);

			var segments = (uri?.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries)) ?? throw new Exception("Invalid URL format.");
			var repo = TrimGitSuffix(segments[^1]);

			return new RepoData
			{
				AbsolutePath = url,
				Name = repo,
				DateRetrieved = DateTime.Now
			};
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error parsing URL '{url}': {ex.Message}");
		}

		throw new Exception("Fatal Error");

		static string TrimGitSuffix(string s) =>
			s.EndsWith(".git", StringComparison.OrdinalIgnoreCase) ? s[..^4] : s;
	}
}
