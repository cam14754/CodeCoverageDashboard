// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard.Services;
public class CommandRunnerService : ICommandRunnerService
{
	public void RunCommand(string fileName, string arguments)
	{
		try
		{
			Process.Start(fileName, arguments);

		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error running command '{fileName} {arguments}': {ex.Message}");
		}
	}
}
