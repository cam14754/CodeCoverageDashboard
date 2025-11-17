// COPYRIGHT © 2025 ESRI
//
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States.
//
// For additional information, contact:
// Environmental Systems Research Institute, Inc.
// Attn: Contracts Dept
// 380 New York Street
// Redlands, California, USA 92373
//
// email: contracts@esri.com

using CodeCoverageDashboard.DTOs;

namespace CodeCoverageDashboard.Interfaces;

public interface IRepoCoverageAnalyzer
{
    static abstract void AnalyzeRepo(RepoData repoData);
    static abstract bool TryParseRepoMetadata(RepoData repoData, CoverageDTO? coverage);
    static abstract bool TryAnalyzeXDocument(RepoData repoData, CoverageDTO? coverage);
}
