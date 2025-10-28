// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard;

public static class SQLQueries
{
	public static string GetLatestRepoRecordsQuery
	{
		get
		{
			//open GetLastRecords.sql and return the contents as a string

			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQLQueries", "GetLatestRepoRecords.sql");
			if (File.Exists(filePath))
			{
				return File.ReadAllText(filePath);
			}

			return string.Empty;
		}
	}
}