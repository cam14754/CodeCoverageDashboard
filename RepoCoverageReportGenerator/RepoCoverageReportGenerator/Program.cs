using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace RepoCoverageReportGenerator;

public class Program
{
	public static async Task Main(string[] args)
	{
		bool isRunning = true;
		bool isBypassConsole = args.Length > 0 && args[0] == "bypass-console";

		while (isRunning)
		{
			string? userInput = "";

			if (isBypassConsole)
			{
				Console.WriteLine("Bypassing human input, proceeding with analysis...");
				userInput = "yes";
			}
			else
			{
				Console.WriteLine("Welcome to the code coverage analyzer");
				Console.WriteLine("Type yes to continue...");
				userInput = Console.ReadLine();
			}
			if (userInput != null && (userInput.Equals("yes", StringComparison.CurrentCultureIgnoreCase) || userInput.Equals("y", StringComparison.CurrentCultureIgnoreCase)))
			{
				var stopwatch = Stopwatch.StartNew();

				try
				{
					Console.WriteLine("Starting code coverage analysis for all repositories");

					string outputDir = Path.Combine(Constants.OutputDir, $"{DateTime.Now:yyyy-MM-dd_HH-mm}_Reports");

					Directory.CreateDirectory(outputDir);

					if (!Directory.Exists(outputDir)) 
					{
						Console.WriteLine($"Failed to create output directory at {outputDir}");
						continue;
					}

					Console.WriteLine($"Generated output directory at {outputDir}");


					foreach (string dir in Directory.GetDirectories(Constants.ReposPath))
					{
						await RepoCoverageReportGenerator.RunAsync(Constants.RunsettingsFilePath, dir, outputDir);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"An error occurred: {ex.Message}");
				}
				finally
				{
					Console.WriteLine("Operation completed. Type 'yes' to run again or 'no' to exit.");
					stopwatch.Stop();
					Console.WriteLine($"All repos tested in {stopwatch.ElapsedMilliseconds} ms.");

					if(isBypassConsole)
					{
						isRunning = false;
					}
				}
			}
			else if (userInput != null && (userInput.Equals("no", StringComparison.CurrentCultureIgnoreCase) || userInput.Equals("n", StringComparison.CurrentCultureIgnoreCase)))
			{
				isRunning = false;
				Console.WriteLine("Exiting the program. Goodbye!");
			}
			else
			{
				Console.WriteLine("Invalid input. Please type 'yes' to continue.");
			}
		}
	}


}

public class RepoCoverageReportGenerator
{
	public static async Task RunAsync(string runSettingsPath, string dir, string outputDir)
	{
		var stopwatch = Stopwatch.StartNew();

		Console.WriteLine($"Starting analysis for {dir}");

		if (RepoIgnore.IgnoreReposList.Contains(dir))
		{
			Console.WriteLine($"Skipping ignored repo: {dir}");
			return;
		}

		

		string srcPath = Path.Combine(dir, "tests");

		if (!Directory.Exists(srcPath))
		{
			Console.WriteLine($"Test folder not found for {dir}");
			return;
		}

		string? childDir = Directory.GetDirectories(srcPath).First();

		if (childDir is null)
		{
			Console.WriteLine($"No child directories found in tests directory for {srcPath}");
			return;
		}

		string? csproj = Directory.GetFiles(childDir, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();

		if (csproj is null)
		{
			Console.WriteLine($"No .csproj file found in {childDir}");
			return;
		}

		Console.WriteLine($"Found .csproj at {csproj}");

		// Build the dotnet test args
		var sb = new StringBuilder();
		sb.Append("test \"")
		  .Append(csproj)
		  .Append("\" ")
		  .Append("--collect:\"XPlat Code Coverage\" ")
		  .Append("--settings:\"").Append(runSettingsPath).Append("\" ")
		  .Append("--results-directory: \"").Append(outputDir).Append("\" ")
		  .Append("--diag: diag.log");


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
			Console.WriteLine($"Failed to start process for {dir}");
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
			Console.WriteLine($"Coverage file not found for {dir}");
			return;
		}

		string lastPart = Path.GetFileName(dir.TrimEnd('\\'));
		string newFilePath = $"{outputDir}\\{lastPart}_CoverageReport.cobertura.xml";
		File.Move(coverageFilePath, newFilePath);

		Console.WriteLine($"Generated coverage report at {newFilePath}");

		foreach (var x in Directory.GetDirectories(outputDir))
		{
			Directory.Delete(x, true); // 'true' means recursive delete
		}

		stopwatch.Stop();
		Console.WriteLine($"Repo {dir} tested in {stopwatch.ElapsedMilliseconds} ms.");
		return;
	}
}

