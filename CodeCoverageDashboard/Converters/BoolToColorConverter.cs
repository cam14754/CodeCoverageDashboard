namespace CodeCoverageDashboard.Converters;

public class BoolToColorConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool Value = (bool)value;

        if (Value) 
        {
            return (Color)Application.Current.Resources["HoveredCellColor"];
            
        } else
        {
            return (Color)Application.Current.Resources["CellColor"];
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
