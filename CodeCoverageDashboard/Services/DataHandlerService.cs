// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace CodeCoverageDashboard.Services;
public class DataHandlerService(IRepoCoverageAnalyzer repoCoverageAnalyzer, IRepoGrabberService repoGrabberService) : IDataHandlerService
{
	public ObservableCollection<RepoData> Repos { get; set; } = [];
	public List<XDocument> CoverageDocuments { get; set; } = [];
	public void LoadReposAsync()
	{
		List<RepoData> repos = repoGrabberService.GetRepoDataAsync();
		Repos.Clear();
		foreach (var repo in repos)
		{
			Repos.Add(repo);
		}
	}

	public async Task TestReposAsync()
	{
		if (Repos.Count == 0)
		{
			throw new Exception("No repositories loaded. Please load repositories before testing.");
		}

		// Path to the results directory
		string resultsDirectoryPath = Path.Combine(FileSystem.AppDataDirectory, ".coverage");

		if (Directory.Exists(resultsDirectoryPath))
		{
			Directory.Delete(resultsDirectoryPath, true); // 'true' means recursive delete
		}
		Directory.CreateDirectory(resultsDirectoryPath);
		Debug.WriteLine("resultsDirectory found/created \n");

		foreach (var repo in Repos)
		{
			try
			{
				await repoCoverageAnalyzer.AnalyzeRepoAsync(repo);

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
	}
}
