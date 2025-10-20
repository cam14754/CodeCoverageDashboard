// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Text.Json;

namespace CodeCoverageDashboard.Services;
public class RepoGrabberService : IRepoGrabberService
{

	static readonly JsonSerializerOptions jsonOptions = new()
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	public List<RepoData> GetRepoDataAsync()
	{
		string? repoDirectory = Environment.GetEnvironmentVariable("REPODIR");

		if (repoDirectory is null)
		{
			throw new Exception("Environment variable 'REPODIR' is not set.");
		}

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
			}
			var data = ParseRepoUrl(raw);
			list.Add(data);

			if (data.IsValid == true)
			{
				Debug.WriteLine($"Parsed: \nURL: {data.AbsolutePath} \nName: {data.Name} \nDate Retrieved: {data.DateRetrieved}\nValid: {data.IsValid}");
			}
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
				IsValid = true,
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
