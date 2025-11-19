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
    public ObservableCollection<StaticDashboardData>? Latest { get; set; }

    public async Task LoadLatestStaticDashboardData()
    {
        
        if (Latest is null || Latest.Count == 0)
        {
            // First load, no cache
            Latest = await databaseService.GetLatestStaticDashboardData();
            Latest.FirstOrDefault()!.DateRetrieved = DateTime.Now;
            return;
        }

        // Cache exists
        var latest = Latest.First();

        if (latest.DateRetrieved < DateTime.Now.AddHours(-12))
        {
            // stale cache, reload
            Latest = await databaseService.GetLatestStaticDashboardData();
            Latest.FirstOrDefault()!.DateRetrieved = DateTime.Now;
            return;
        }

        // Cache is still fresh, do nothing
    }

    public bool ClearCache()
    {
        if(Latest is null || Latest.Count is 0)
        {
            return false;
        }

        Latest.Clear();
        Latest = null;
        return true;
    }
}

