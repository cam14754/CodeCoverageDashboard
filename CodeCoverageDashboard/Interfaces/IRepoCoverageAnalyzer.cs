// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using CodeCoverageDashboard.DTOs;

namespace CodeCoverageDashboard.Interfaces;

public interface IRepoCoverageAnalyzer
{
	static abstract void AnalyzeRepo(RepoData repoData);
	static abstract bool TryParseRepoMetadata(RepoData repoData, CoverageDTO? coverage);
	static abstract bool TryAnalyzeXDocument(RepoData repoData, CoverageDTO? coverage);
}