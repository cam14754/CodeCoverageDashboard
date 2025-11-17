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

namespace CodeCoverageDashboard.ViewModels;

[QueryProperty(nameof(SelectedRepo), nameof(SelectedRepo))]
public partial class RepoPageViewModel : BaseViewModel
{
    [ObservableProperty]
    public partial RepoData SelectedRepo { get; set; }

    partial void OnSelectedRepoChanged(RepoData value)
    {
        Title = value?.Name ?? "Repository Details";
    }

    [RelayCommand]
    public async Task GoToClassPageAsync(ClassData classData)
    {
        if (classData is null)
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
            await Shell.Current.GoToAsync(nameof(ClassPage), true, new Dictionary<string, object>
            {
                { "SelectedClass", classData }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error navigating to ClassPage: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
