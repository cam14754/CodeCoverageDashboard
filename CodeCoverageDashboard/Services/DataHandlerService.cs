// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace CodeCoverageDashboard.Services;
public class DataHandlerService(IDatabaseService databaseService) : IDataHandlerService
{
	public ObservableCollection<RepoData> Repos { get; set; } = [];

	public async Task GetXDocRequest()
	{
		Repos.Clear();
		List<XDocument> xDocsFromService = await HTTPService.GetXDocs();

		foreach (var xDoc in xDocsFromService)
		{
			RepoData newRepo = new RepoData { XDocument = xDoc };
			RepoCoverageAnalyzer.AnalyzeRepo(newRepo);
			Repos.Add(newRepo);
		}

		await databaseService.SaveMemoryToDB([.. Repos]);

		Repos.Clear();

		var repos = await databaseService.LoadLatestReposList();
		foreach (var repo in repos)
		{
			Repos.Add(repo);
		}
	}
}
