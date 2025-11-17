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

public class ZeroToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // handle nulls and non-numeric values safely
        if (value == null)
        {
            return false;
        }

        if (value is double d)
        {
            return Math.Abs(d) < 0.0001;   // true if 0
        }

        if (value is int i)
        {
            return i == 0;
        }

        if (value is float f)
        {
            return Math.Abs(f) < 0.0001;
        }

        // fallback
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
