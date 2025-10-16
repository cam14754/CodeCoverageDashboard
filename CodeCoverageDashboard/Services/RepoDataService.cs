// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Services;
public class RepoDataService : IRepoDataService
{
	public async Task<bool> GetRepoDataAsync(string repoUrl)
	{
		// Simulate an asynchronous operation
		await Task.Delay(500);
		Debug.WriteLine($"Fetched data for repo: {repoUrl}");
		return true;
	}
}
