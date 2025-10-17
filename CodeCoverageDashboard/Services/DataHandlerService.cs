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
		List<XDocument> coverageDocuments = await repoCoverageAnalyzer.AnalyzeRepoAsync();
		foreach (var coverageDocument in coverageDocuments)
		{
			Debug.WriteLine(coverageDocument.ToString());
		}
	}
}
