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
			RepoData previousRepo = await databaseService.LoadLatestRepoByName(newRepo.Name);
			if (previousRepo is null)
			{
				newRepo.CoveragePercentPercentIncrease = newRepo.CoveragePercent;
			}
			else
			{
				if(previousRepo.CoveragePercent == 0)
				{
					newRepo.CoveragePercentPercentIncrease = newRepo.CoveragePercent;
				}
				else
				{
					newRepo.CoveragePercentPercentIncrease = ((newRepo.CoveragePercent - previousRepo.CoveragePercent) / previousRepo.CoveragePercent) * 100;
				}
			}
			await databaseService.SaveMemoryToDB(newRepo);
		}

		var repos = await databaseService.LoadLatestReposList();
		foreach (var repo in repos)
		{
			Repos.Add(repo);
		}
	}
}