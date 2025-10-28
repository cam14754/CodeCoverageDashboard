// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using CodeCoverageDashboard.Converters;
using CodeCoverageDashboard.Tables;
using SQLite;


namespace CodeCoverageDashboard.Services;
public class DatabaseService : IDatabaseService
{
	public DatabaseService()
	{
		_ = Init();
	}

	SQLiteAsyncConnection database;

	async Task Init()
	{
		if (database is not null)
		{
			return;
		}

		database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
		await CreateTables();
	}

	async Task CreateTables()
	{
		await database.CreateTableAsync<RepoRecord>();
		await database.CreateTableAsync<MethodTable>();
		await database.CreateTableAsync<DashboardTable>();
	}

	async Task IDatabaseService.SaveMemoryToDB(List<RepoData> repoDatas)
	{
		await Init();
		foreach (var repo in repoDatas)
		{
			var x = RepoRecordToRepoObject.ConvertBack(repo);
			await database.InsertAsync(x);
		}
	}

	public async Task<List<RepoData>> LoadLatestReposList()
	{
		await Init();
		var RepoRecords = await database.QueryAsync<RepoRecord>(SQLQueries.GetLatestRepoRecordsQuery);
		var RepoObjects = new List<RepoData>();
		foreach (var RepoRecord in RepoRecords)
		{
			var RepoObject = (RepoData)RepoRecordToRepoObject.Convert(RepoRecord);
			RepoObjects.Add(RepoObject);
		}
		return RepoObjects;
	}
}
