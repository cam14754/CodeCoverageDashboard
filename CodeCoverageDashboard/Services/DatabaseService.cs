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
        await database.CreateTableAsync<DashboardRecord>();
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
            Debug.WriteLine($"Error reading SQL file {fileName}: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task<ObservableCollection<StaticDashboardData>> GetLatestStaticDashboardData()
    {
        await Init();

        var DashboardDataRecord = await database.QueryAsync<DashboardRecord>(await ReadSQLAsync("GetAllStaticdashboardData.sql"));

        if(DashboardDataRecord is null || DashboardDataRecord.FirstOrDefault() is null)
        {
            Debug.WriteLine("No dashboard data found / DB call failed.");
            return [];
        }

        var CollectionDashboardData = new ObservableCollection<StaticDashboardData>();

        foreach(var item in DashboardDataRecord)
        {
            var ConvertedData = (StaticDashboardData)StaticDashboardRecordToStaticDashboardObject.Convert(item);

            if(ConvertedData is null)
            {
                Debug.WriteLine("Failed to convert dashboard data record to dashboard data object.");
            }

            CollectionDashboardData.Add(ConvertedData);
        }

        return CollectionDashboardData;
    }
}
