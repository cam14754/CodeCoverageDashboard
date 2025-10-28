// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard;
public static class Constants
{
	public const string DatabaseFilename = "CodeCoverageDataBase.db";

	public const SQLite.SQLiteOpenFlags Flags =
		// open the database in read/write mode
		SQLite.SQLiteOpenFlags.ReadWrite |
		// create the database if it doesn't exist
		SQLite.SQLiteOpenFlags.Create |
		// enable multi-threaded database access
		SQLite.SQLiteOpenFlags.SharedCache;

	public static string DatabasePath => $"C:\\Users\\cam14754\\Desktop\\UnitTestingInternProject\\CodeCoverageDashboard\\CodeCoverageDashboard\\{DatabaseFilename}";
}

