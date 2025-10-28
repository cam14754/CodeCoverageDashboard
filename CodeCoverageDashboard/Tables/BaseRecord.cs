// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Tables;

public abstract class BaseRecord<T>
{
	[Column("id")]
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }

	[Column("date_retrieved")]
	public DateTime DateRetrieved { get; set; } = DateTime.MinValue;

	[Column("properties")]
	public string PropertiesText { get; set; } = string.Empty;

	[Ignore]
	public T? Properties
	{
		get => JsonSerializer.Deserialize<T>(this.PropertiesText);
		set => this.PropertiesText = JsonSerializer.Serialize(value);
	}
}