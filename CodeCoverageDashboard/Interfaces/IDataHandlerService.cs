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

using System.Collections.ObjectModel;

namespace CodeCoverageDashboard.Interfaces;

public interface IDataHandlerService
{
    public ObservableCollection<StaticDashboardData>? Latest { get; set; }
    abstract Task LoadLatestStaticDashboardData();

    public bool ClearCache();
}
