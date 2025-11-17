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

