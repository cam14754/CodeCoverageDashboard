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

public class CoveragePercentToColorGradient : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double Value = (double)value;
        // Soft max value to reduce vibrance
        const double Max = 200.0;

        double r, g;

        if (Value <= 0.5)
        {
            // 0 → 0.5 = red (180,0,0) → yellow (180,180,0)
            // green goes 0 → 180
            r = Max;
            g = (Value / 0.5) * Max;
        }
        else
        {
            // 0.5 → 1 = yellow (180,180,0) → green (0,180,0)
            // red goes 180 → 0
            r = ((1.0 - Value) / 0.5) * Max;
            g = Max;
        }

        return Color.FromRgb((int)r, (int)g, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();


}
