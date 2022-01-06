using System.Globalization;

namespace VideoDownloader.Converters;

class SizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is not long size ? Binding.DoNothing : size switch
        {
            < 1024 => $"{size}B",
            < 1024L * 1024 => $"{size / 1024}KB",
            < 1024L * 1024 * 1024 => $"{size / (1024L * 1024L)}MB",
            < 1024L * 1024 * 1024 * 1024 => $"{size / (1024L * 1024L * 1024L)}GB",
            _ => $"{size / (1024L * 1024L * 1024L * 1024L)}TB"
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
