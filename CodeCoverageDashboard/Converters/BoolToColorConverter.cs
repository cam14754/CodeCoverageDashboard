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

namespace CodeCoverageDashboard.Converters;

public class BoolToColorConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool Value = (bool)value;

        if (Value)
        {
            return (Color)Application.Current.Resources["HoveredCellColor"];

        }
        else
        {
            return (Color)Application.Current.Resources["CellColor"];
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
