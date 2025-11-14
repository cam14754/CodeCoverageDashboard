// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

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