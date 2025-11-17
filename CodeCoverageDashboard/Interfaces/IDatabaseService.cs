// COPYRIGHT © 2025 ESRI
//
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States.
//
// For additional information, contact:
// Environmental Systems Research Institute, Inc.
// Attn: Contracts Dept
// 380 New York Street
// Redlands, California, USA 92373
//
// email: contracts@esri.com

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
