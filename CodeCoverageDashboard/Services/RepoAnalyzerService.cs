// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Services;
public class RepoAnalyzerService : IRepoAnalyzerService
{


	readonly ICommandRunnerService commandRunnerService;

	public RepoAnalyzerService(ICommandRunnerService commandRunnerService)
	{
		this.commandRunnerService = commandRunnerService;
	}


	public void AnalyzeRepo(string repoPath)
	{
		commandRunnerService.RunCommand("cmd.exe", "tree");
	}
}
