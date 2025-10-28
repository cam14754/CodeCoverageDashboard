// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Tables;

[Table("Methods")]
public class MethodRecord : BaseRecord<MethodProperties>
{
	[Column("repo_name")]
	public string RepoName { get; set; } = string.Empty;
}