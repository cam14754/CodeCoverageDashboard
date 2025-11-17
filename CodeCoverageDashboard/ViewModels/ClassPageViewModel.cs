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

[QueryProperty(nameof(SelectedClass), nameof(SelectedClass))]
public partial class ClassPageViewModel : BaseViewModel
{
    [ObservableProperty]
    public partial ClassData SelectedClass { get; set; }

    partial void OnSelectedClassChanged(ClassData value)
    {
        Title = value?.Name ?? "Class Details";
    }
}
