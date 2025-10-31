namespace CodeCoverageDashboard.Converters;

public static class RepoRecordToRepoObject
{
	//RepositoryRecord Record to RepoData Object Converter
	public static object? Convert(object? value)
	{
		var x = new RepoData();
		var repoRecord = (RepoRecord)value ?? throw new NullReferenceException("RepoRecord is null in RepoRecordToRepoObject converter.");

		x.Name = repoRecord.RepoName;
		x.DateRetrieved = repoRecord.DateRetrieved;

		var properties = repoRecord.Properties ?? throw new NullReferenceException("RepoRecord.Properties is null in RepoRecordToRepoObject converter.");

		x.CoveredLines = properties.CoveredLines;
		x.TotalLines = properties.NumLines;
		x.CoveragePercent = properties.CoveragePercent;
		x.ListClasses = properties.Classes;
		x.CoveragePercentPercentIncrease = properties.CoveragePercentPercentIncrease;

		return x;
	}

	//RepoData Object to RepositoryRecord Record Converter
	public static object? ConvertBack(object? value)
	{
		var x = new RepoRecord();
		var repoData = (RepoData)value ?? throw new NullReferenceException("RepoData is null in RepoRecordToRepoObject converter.");

		x.RepoName = repoData.Name;
		x.DateRetrieved = repoData.DateRetrieved;
		x.Properties = new RepoProperties
		{
			RepoName = repoData.Name,
			DateRetrieved = repoData.DateRetrieved,
			CoveredLines = repoData.CoveredLines,
			NumLines = repoData.TotalLines,
			CoveragePercent = repoData.CoveragePercent,
			UncoveredLines = repoData.UncoveredLines,
			Classes = repoData.ListClasses,
			CoveragePercentPercentIncrease = repoData.CoveragePercentPercentIncrease
		};

		return x;
	}
}