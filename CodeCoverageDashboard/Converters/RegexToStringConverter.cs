namespace CodeCoverageDashboard.Converters;
    public class RegexToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && parameter is string pattern)
                return Regex.Replace(s, pattern, string.Empty);

            return value;
        }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
