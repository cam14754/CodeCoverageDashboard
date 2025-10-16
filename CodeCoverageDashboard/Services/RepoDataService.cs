// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Text.Json;

namespace CodeCoverageDashboard.Services;
public class RepoDataService : IRepoDataService
{

	static readonly JsonSerializerOptions jsonOptions = new()
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	public List<RepoData>? GetRepoDataAsync(CancellationToken ct = default)
	{
		var lines = RepoURLs.Urls;
		var workRoot = Path.Combine(FileSystem.AppDataDirectory, "repos");
		Directory.CreateDirectory(workRoot);

		var list = new List<RepoData>();

		foreach (var raw in lines)
		{
			ct.ThrowIfCancellationRequested();

			var data = ParseRepoUrl(raw, workRoot);
			list.Add(data);

			if (data.IsValid == true)
			{
				Debug.WriteLine($"Parsed: \nURL: {data.Url} \nName: {data.Name} \nOrg: {data.Org} \nLocalPath: {data.LocalPath} \nDate Retrieved: {data.DateRetrieved}\n");
			}
			else
			{
				Debug.WriteLine($"Invalid URL '{raw}': {data.Error}");
			}
		}

		return list;
	}

	static RepoData ParseRepoUrl(string url, string workRoot)
	{
		try
		{
			Uri.TryCreate(url, UriKind.Absolute, out var uri);

			var segments = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

			var org = segments[^2];
			var repo = TrimGitSuffix(segments[^1]);

			var local = Path.Combine(workRoot, repo);
			return new RepoData
			{
				Url = url,
				Name = repo,
				Org = org,
				LocalPath = local,
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
