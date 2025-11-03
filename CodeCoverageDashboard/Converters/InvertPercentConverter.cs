

namespace CodeCoverageDashboard.Converters;

public class InvertPercentConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is double d ? (double)((double)1 - (double)d) : (object)0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is double d ? (double)((double)1 - (double)d) : (object)0;
	}
}
