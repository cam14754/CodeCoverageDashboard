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

public class DataHandlerService(IDatabaseService databaseService) : IDataHandlerService
{
    private ObservableCollection<StaticDashboardData>? Data { get; set; }

    private async Task LoadLatestStaticDashboardData()
    {
        
        if (Data is null || Data.Count == 0)
        {
            // First load, no cache
            Data = await databaseService.GetLatestStaticDashboardData();
            foreach(StaticDashboardData dash in Data)
            {
                dash.DateRetrieved = DateTime.Now;
            }
            return;
        }

        // Cache exists
        var latest = Data.First();

        if (latest.DateRetrieved < DateTime.Now.AddHours(-12))
        {
            // stale cache, reload
            Data = await databaseService.GetLatestStaticDashboardData();
            foreach (StaticDashboardData dash in Data)
            {
                dash.DateRetrieved = DateTime.Now;
            }
            return;
        }

        // Cache is still fresh, do nothing
    }

    public bool ClearMemory()
    {
        if(Data is null || Data.Count is 0)
        {
            return false;
        }

        Data.Clear();
        Data = null;
        return true;
    }

    public async Task<StaticDashboardData> GetWeekOldData()
    {
        await LoadLatestStaticDashboardData();

        var target = DateTime.Now.AddDays(-7);
        var start = target.AddHours(-6);
        var end = target.AddHours(6);
        StaticDashboardData? WeekOldData = Data?.Where(d => d.DateRetrieved >= start && d.DateRetrieved <= end).FirstOrDefault();

        if(WeekOldData is null)
        {
            return new StaticDashboardData();
        }

        return WeekOldData;
    }

    public async Task<StaticDashboardData> GetLatestData()
    {
        await LoadLatestStaticDashboardData();

        StaticDashboardData? LatestData = Data?.MaxBy(d => d.DataAge);

        if (LatestData is null)
        {
            return new StaticDashboardData();
        }

        return LatestData;
    }

    public async Task<ObservableCollection<StaticDashboardData>> GetAllData()
    {
        await LoadLatestStaticDashboardData();

        if(Data is null)
        {
            return new ObservableCollection<StaticDashboardData>();
        }

        return Data;
    }
}

