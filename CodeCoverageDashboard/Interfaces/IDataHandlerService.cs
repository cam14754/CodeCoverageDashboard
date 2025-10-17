// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Collections.ObjectModel;

namespace CodeCoverageDashboard.Interfaces;
public interface IDataHandlerService
{
	ObservableCollection<RepoData> Repos { get; set; }
	void LoadReposAsync();
	Task TestReposAsync();
}