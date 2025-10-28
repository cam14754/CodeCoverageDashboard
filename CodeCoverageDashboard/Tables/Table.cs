// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Text.Json;
using SQLite;
using static CodeCoverageDashboard.Models.DTOs;

namespace CodeCoverageDashboard.Tables;


public abstract class BaseRecord
{
	[Column("id")]
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }

	[Column("repo_name")]
	public string RepoName { get; set; } = string.Empty;

	[Column("date_retrieved")]
	public DateTime DateRetrieved { get; set; } = DateTime.MinValue;

	[Column("properties")]
	public string PropertiesText { get; set; } = string.Empty;
}


[Table("Repos")]
class RepoRecord : BaseRecord
{
	[Ignore]
	public RepoProperties? Properties
	{
		get => JsonSerializer.Deserialize<RepoProperties>(PropertiesText);
		set => PropertiesText = JsonSerializer.Serialize(value);
	}
}

[Table("Methods")]
class MethodTable : BaseRecord
{
	[Ignore]
	public MethodProperties? Properties
	{
		get => JsonSerializer.Deserialize<MethodProperties>(PropertiesText);
		set => PropertiesText = JsonSerializer.Serialize(value);
	}
}

[Table("DashboardTable")]
class DashboardTable : BaseRecord
{
	[SQLite.Ignore]
	public DashboardProperties? Properties
	{
		get => JsonSerializer.Deserialize<DashboardProperties>(PropertiesText);
		set => PropertiesText = JsonSerializer.Serialize(value);
	}
}
