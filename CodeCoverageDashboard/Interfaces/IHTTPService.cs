// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.

using System.Xml.Linq;

namespace CodeCoverageDashboard.Interfaces;
public interface IHTTPService
{
	static abstract Task<List<XDocument>> GetXDocs();
}