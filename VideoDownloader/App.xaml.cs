namespace VideoDownloader;

public partial class App : Application
{
    public static YoutubeDL YoutubeDL { get; } = new YoutubeDL
    {
        FFmpegPath = "runtime/ffmpeg.exe",
        OverwriteFiles = true,
        YoutubeDLPath = "runtime/yt-dlp.exe"
    };

    protected override void OnStartup(StartupEventArgs e)
    {
        SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
    }
}
