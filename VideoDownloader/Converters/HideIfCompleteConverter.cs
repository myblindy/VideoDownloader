using System.Globalization;

namespace VideoDownloader.Converters;

class HideIfCompleteConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = value switch
        {
            DownloadState downloadState => downloadState is DownloadState.Error or DownloadState.Success,
            bool => true,
            _ => false
        };

        if (Invert) result = !result;

        return result ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
