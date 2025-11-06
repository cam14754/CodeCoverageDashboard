// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Interfaces;
public interface IDatabaseService
{
	abstract Task SaveMemoryToDB(List<RepoData> repoDatas);
	abstract Task SaveMemoryToDB(RepoData repoData);
	abstract Task SaveMemoryToDB(StaticDashboardData staticDashboardData);

	abstract Task<List<RepoData>> LoadLatestReposList();
	abstract Task<RepoData> LoadLatestRepoByName(string repoName);
	abstract Task<StaticDashboardData> LoadXWeekOldDashboardData(int x);
	abstract Task<List<StaticDashboardData>> LoadAllDashboardData();
	abstract Task<List<RepoData>> LoadAllRepoData();

	abstract Task<StaticDashboardData> LoadSecondLatestDashboardData();


}