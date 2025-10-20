// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

namespace CodeCoverageDashboard;
using System.Diagnostics;

public static class TraceConfig
{
	public static void Setup()
	{
		Trace.Listeners.Clear();
		Trace.Listeners.Add(new TextWriterTraceListener("trace.log"));
		Trace.Listeners.Add(new ConsoleTraceListener());
		Trace.AutoFlush = true;

		Trace.WriteLine("Trace system initialized!");
	}
}

