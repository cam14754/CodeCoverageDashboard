// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.
namespace CodeCoverageDashboard.Services;

using CodeCoverageDashboard.Converters;
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
		await database.CreateTableAsync<DashboardRecord>();
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

	async Task IDatabaseService.SaveMemoryToDB(RepoData repoData)
	{
		await Init();
		var x = RepoRecordToRepoObject.ConvertBack(repoData);
		await database.InsertAsync(x);
	}

	async Task IDatabaseService.SaveMemoryToDB(List<StaticDashboardData> staticDashboardDatas)
	{
		await Init();
		foreach (var repo in staticDashboardDatas)
		{
			var x = StaticDashboardRecordToStaticDashboardObject.ConvertBack(repo);
			await database.InsertAsync(x);
		}
	}

	async Task IDatabaseService.SaveMemoryToDB(StaticDashboardData staticDashboardData)
	{
		await Init();
		var x = StaticDashboardRecordToStaticDashboardObject.ConvertBack(staticDashboardData);
		await database.InsertAsync(x);
	}

	public async Task<List<RepoData>> LoadLatestReposList()
	{
		await Init();
		var RepoRecords = await database.QueryAsync<RepoRecord>(await ReadSQLAsync("GetLatestRepoRecords.sql"));
		var RepoObjects = new List<RepoData>();
		foreach (var RepoRecord in RepoRecords)
		{
			var RepoObject = (RepoData)RepoRecordToRepoObject.Convert(RepoRecord);
			RepoObjects.Add(RepoObject);
		}
		return RepoObjects;
	}

	public async Task<StaticDashboardData> LoadXWeekOldDashboardData(int x)
	{
		await Init();
		var DashboardData = await database.QueryAsync<DashboardRecord>(await ReadSQLAsync("GetXWeekOldDashboardRecords.sql"), x);
		return (StaticDashboardData)StaticDashboardRecordToStaticDashboardObject.Convert(DashboardData);
		
	}

	public static async Task<string> ReadSQLAsync(string fileName)
	{
		try
		{
			using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
			using var reader = new StreamReader(stream);
			return await reader.ReadToEndAsync();
		}
		catch (Exception ex)
		{
			throw new IOException($"Failed to load SQL file: {fileName}", ex);
		}
	}
}