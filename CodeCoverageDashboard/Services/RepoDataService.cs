// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Services;
public class RepoDataService : IRepoDataService
{
	static readonly string repoListPaths = Path.Combine(FileSystem.AppDataDirectory, "repos.txt");
	public async Task<bool> GetRepoDataAsync()
	{
		// Simulate an asynchronous operation
		await Task.Delay(500);
		Debug.WriteLine("Fetching repo data from file");

		if (!File.Exists(repoListPaths))
		{
			Debug.WriteLine($"Repo list file not found at {repoListPaths}");
			return false;
		}

		var repoUrls = await File.ReadAllLinesAsync(repoListPaths);

		foreach (var url in repoUrls)
		{
			Debug.WriteLine($"Found repo URL: {url}");
		}

		return true;
	}
}
