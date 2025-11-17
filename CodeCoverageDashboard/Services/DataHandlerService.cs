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
    public ObservableCollection<RepoData> Repos { get; set; } = [];

    public async Task ProcessXDocsFromHTTP()
    {
        Repos.Clear();

        List<XDocument> xDocsFromService = await HTTPService.GetXDocs();

        foreach (var xDoc in xDocsFromService)
        {
            RepoData newRepo = new() { XDocument = xDoc };

            RepoCoverageAnalyzer.AnalyzeRepo(newRepo);

            Repos.Add(newRepo);
        }
    }
}
