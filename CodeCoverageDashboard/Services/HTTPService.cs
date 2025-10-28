// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

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
		return await Task.Run(() =>
		{
			return Directory.GetFiles("C:\\Users\\cam14754\\Desktop\\Reports", "*.cobertura.xml").Select(XDocument.Load).ToList();
		});
	}
}