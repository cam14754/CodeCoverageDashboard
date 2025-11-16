// SPDX-License-Identifier: Proprietary
// © 2025 Cameron Strachan, trading as Cameron's Rock Company. All rights reserved.
// Created by Cameron Strachan.
// For personal and educational use only.
namespace CodeCoverageDashboard.ViewModels;
public partial class DrillDownDashboardPageViewModel(IDataHandlerService dataHandlerService) : BaseViewModel
{
	public ObservableCollection<RepoData> Repos { get; set; }
    public ObservableCollection<RepoData> VisableRepos { get; set; }

	[ObservableProperty]
	public partial double TotalLines { get; set; }
    [ObservableProperty]
    public partial double AverageCoveragePercent { get; set; }

	public string DashboardVersion { get; set; } = Constants.DashboardVersion;

    readonly IDataHandlerService datahandlerService = dataHandlerService;

	[RelayCommand]
	public async Task RunAsync()
	{
		if (IsBusy)
		{
			return;
		}
		try
		{
			IsBusy = true;
			Debug.WriteLine("Loading repos... \n");

			//Get from HTTP
			await datahandlerService.ProcessXDocsFromHTTP();

			//Save to memory
			Repos = dataHandlerService.Repos;
			VisableRepos = Repos;

			UpdateStats();

			OnPropertyChanged(nameof(VisableRepos));

			Debug.WriteLine("Repos loaded successfully.");

		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading repos: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public async Task GoToRepoPageAsync(RepoData repoData)
	{
		if (repoData is null)
		{
			return;
		}
		if (IsBusy)
		{
			return;
		}
		IsBusy = true;
		try
		{
			await Shell.Current.GoToAsync(nameof(RepoPage), true, new Dictionary<string, object>
			{
				{ "SelectedRepo", repoData }
			});
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error navigating to RepoPage: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public async Task GoToStaticDashboardPage()
	{
		if (IsBusy)
		{
			return;
		}
		IsBusy = true;
		try
		{
			await Shell.Current.GoToAsync($"//{nameof(StaticDashboardPage)}");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error navigating to StaticDashboardPage: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public void UpdateStats()
	{
		TotalLines = VisableRepos?.Sum(r => r.TotalLines) ?? 0;
		AverageCoveragePercent = VisableRepos?.Average(r => r.CoveragePercent) ?? 0;
	}
}