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
		Directory.Delete(Path.Combine(FileSystem.AppDataDirectory, "coverage"), true);
		var watch = Stopwatch.StartNew();

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

		watch.Stop();
		Debug.WriteLine($"\nFull analysis finished in {(double)((double)watch.ElapsedMilliseconds / 1000d)} seconds");
	}
}
