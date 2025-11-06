// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.Specialized;

namespace CodeCoverageDashboard.Services;

class HTTPService : IHTTPService
{
	public static async Task<List<XDocument>> GetXDocs()
	{
		//List<RepoData> repoDatas = GetRepoDataAsync();
		//foreach (RepoData repodata in repoDatas)
		//{
		//	await AnalyzeRepoAsync(repodata);
		//}

		//return [.. repoDatas.Select(x => x.XDocument)];
		return await getfromdesk();
	}

	public static async Task<List<XDocument>> getfromdesk()
	{
		await Task.Run(() =>
		{
			Thread.Sleep(100); // Simulate async work
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

		if (latestFolder == null)
		{
			throw new Exception("No valid folders found in the specified directory.");
		}

		return Directory.GetFiles(latestFolder.Path, "*.cobertura.xml").Select(XDocument.Load).ToList();

	}
}