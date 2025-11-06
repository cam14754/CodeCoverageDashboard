// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

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
			RepoData newRepo = new() { XDocument = xDoc };

			RepoCoverageAnalyzer.AnalyzeRepo(newRepo);
			
			await databaseService.SaveMemoryToDB(newRepo);

			Repos.Add(newRepo);
		}
	}
}