using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace RepoCoverageReportGenerator;

public class Program
{
	public static async Task Main(string[] args)
	{		
		var stopwatch = Stopwatch.StartNew();

		try
		{
			Console.WriteLine("\nStarting code coverage analysis for all repositories");

			string outputDir = Path.Combine(Constants.OutputDir, $"{DateTime.Now:yyyy-MM-dd_HH-mm}_Reports");

			Directory.CreateDirectory(outputDir);

			if (!Directory.Exists(outputDir))
			{
				Debug.WriteLine($"Failed to create output directory at {outputDir}");
				return;
			}

			Console.WriteLine($"Generated output directory at {outputDir}");


			foreach (string dir in Directory.GetDirectories(Constants.ReposPath))
			{
				await RepoCoverageReportGenerator.RunAsync(Constants.RunsettingsFilePath, dir, outputDir);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"An error occurred: {ex.Message}");
		}
		finally
		{
			Console.WriteLine("Operation completed.");
			stopwatch.Stop();
			Debug.WriteLine($"Operation completed in {stopwatch.ElapsedMilliseconds} ms.");
		}
	}

public class RepoCoverageReportGenerator
{
	public static async Task RunAsync(string runSettingsPath, string dir, string outputDir)
	{
		var stopwatch = Stopwatch.StartNew();

		Debug.WriteLine($"Starting analysis for {dir}");

		if (RepoIgnore.IgnoreReposList.Contains(dir))
		{
			Debug.WriteLine($"Skipping ignored repo: {dir}");
			return;
		}



		string srcPath = Path.Combine(dir, "tests");

		if (!Directory.Exists(srcPath))
		{
			Debug.WriteLine($"Test folder not found for {dir}");
			return;
		}

		string? childDir = Directory.GetDirectories(srcPath).First();

		if (childDir is null)
		{
			Debug.WriteLine($"No child directories found in tests directory for {srcPath}");
			return;
		}

		string? csproj = Directory.GetFiles(childDir, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();

		if (csproj is null)
		{
			Debug.WriteLine($"No .csproj file found in {childDir}");
			return;
		}

		Debug.WriteLine($"Found .csproj at {csproj}");

		// Build the dotnet test args
		var sb = new StringBuilder();
		sb.Append("test \"")
		  .Append(csproj)
		  .Append("\" ")
		  .Append("--collect:\"XPlat Code Coverage\" ")
		  .Append("--settings:\"").Append(runSettingsPath).Append("\" ")
		  .Append("--results-directory: \"").Append(outputDir).Append("\" ");


		string args = sb.ToString();

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

		Console.WriteLine($"Starting dotnet test for {dir}...");
		if (!process.Start())
		{
			Debug.WriteLine($"Failed to start process for {dir}");
		}

		var stdOutTask = process.StandardOutput.ReadToEndAsync();
		var stdErrTask = process.StandardError.ReadToEndAsync();

		await process.WaitForExitAsync();

		var stdout = await stdOutTask;
		var stderr = await stdErrTask;

		if (process.ExitCode != 0)
		{
			Console.WriteLine($"dotnet test failed for {dir} (exit code:{process.ExitCode})");
			Console.WriteLine($"Standard Output: {stdout}");
			Console.WriteLine($"Error Output: {stderr}");
			return;
		}

		Console.WriteLine($"dotnet test completed successfully for {dir}");

		// Find the Cobertura coverage file that was generated
		string? coverageFilePath = Directory.GetFiles(outputDir, "coverage.cobertura.xml", SearchOption.AllDirectories).FirstOrDefault();

		if (coverageFilePath is null)
		{
			Debug.WriteLine($"Coverage file not found for {dir}");
			return;
		}

		string lastPart = Path.GetFileName(dir.TrimEnd('\\'));
		string newFilePath = $"{outputDir}\\{lastPart}_CoverageReport.cobertura.xml";
		File.Move(coverageFilePath, newFilePath);

		Debug.WriteLine($"Generated coverage report at {newFilePath}");

		foreach (var x in Directory.GetDirectories(outputDir))
		{
			Directory.Delete(x, true); // 'true' means recursive delete
		}

		stopwatch.Stop();
		Debug.WriteLine($"Repo {dir} tested in {stopwatch.ElapsedMilliseconds} ms.");
		return;
		}
	}
}
