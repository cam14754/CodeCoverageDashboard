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

using System.Collections.Specialized;

namespace CodeCoverageDashboard.Services;

public class HTTPService : IHTTPService
{
    public static async Task<List<XDocument>> GetXDocs()
    {

        return await Getfromdesk();
    }

    public static async Task<List<XDocument>> Getfromdesk()
    {
        await Task.Run(() =>
        {
            Thread.Sleep(1); // Simulate async work
        });

        //Collect data from the desktop, from the folder which name is the latest cronologically

        string folder = "C:\\Users\\cam14754\\Desktop\\Reports";
        string[] dirs = Directory.GetDirectories(folder);

        // Select the folder with the latest timestamp in its name
        var latestFolder = dirs
            .Select(path => new
            {
                Path = path,
                // Parse folder name into DateTime from yyyy-MM-dd-HH-mm format
                Date = DateTime.TryParseExact(
                    Path.GetFileName(path),
                    "yyyy-MM-dd-HH-mm",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime dt)
                    ? dt
                    : DateTime.MinValue
            })
            .OrderByDescending(x => x.Date)
            .FirstOrDefault();

        return latestFolder is null
            ? throw new Exception("No valid folders found in the specified directory.")
            : Directory.GetFiles(latestFolder.Path, "*.cobertura.xml").Select(XDocument.Load).ToList();
    }
}
