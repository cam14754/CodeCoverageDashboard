namespace CodeCoverageDashboard.Converters;

public class CoveragePercentToColorGradient : IValueConverter
{
    // You can tweak these stops however you like
    // position: 0–1 range
    private readonly List<(double position, Color color)> _stops =
    [
        (0.00, Colors.Red),
        (0.30, Colors.Red),
        (0.39, Colors.Yellow),
        (0.50, Colors.Green),
        (1.00, Colors.Green)
    ];

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return Colors.Transparent;

        double t;

        if (value is double d)
            t = d;
        else if (value is float f)
            t = f;
        else if (value is int i)
            t = i;
        else
            return Colors.Transparent;

        // Support 0–100 too
        if (t > 1.0)
            t /= 100.0;

        t = Math.Clamp(t, 0.0, 1.0);

        // Edge cases
        if (t <= _stops[0].position)
            return _stops[0].color;

        if (t >= _stops[^1].position)
            return _stops[^1].color;

        // Find the two stops we're between
        for (int i = 0; i < _stops.Count - 1; i++)
        {
            var (posA, colorA) = _stops[i];
            var (posB, colorB) = _stops[i + 1];

            if (t >= posA && t <= posB)
            {
                double localT = (t - posA) / (posB - posA);
                return LerpColor(colorA, colorB, localT);
            }
        }

        // Fallback (shouldn't hit)
        return _stops[^1].color;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    private static Color LerpColor(Color a, Color b, double t)
    {
        t = Math.Clamp(t, 0.0, 1.0);

        return new Color(
            (float)(a.Red + (b.Red - a.Red) * t),
            (float)(a.Green + (b.Green - a.Green) * t),
            (float)(a.Blue + (b.Blue - a.Blue) * t),
            (float)(a.Alpha + (b.Alpha - a.Alpha) * t));
    }
}
