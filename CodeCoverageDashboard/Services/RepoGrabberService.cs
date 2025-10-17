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
		List<string> lines = RepoURLs.Urls;

		var list = new List<RepoData>();

		foreach (var raw in lines)
		{
			var data = ParseRepoUrl(raw);
			list.Add(data);

			if (data.IsValid == true)
			{
				Debug.WriteLine($"Parsed: \nURL: {data.Url} \nName: {data.Name} \nOrg: {data.Org} \nDate Retrieved: {data.DateRetrieved}\nValid: {data.IsValid}");
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
			var org = segments?[^2];
			var repo = TrimGitSuffix(segments?[^1]);

			return new RepoData
			{
				Url = url,
				Name = repo,
				Org = org,
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
