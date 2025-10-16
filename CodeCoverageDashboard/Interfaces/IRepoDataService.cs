// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Interfaces;
public interface IRepoDataService
{
	Task<List<RepoData>> GetRepoDataAsync(string repoPathList, CancellationToken ct = default);
}
