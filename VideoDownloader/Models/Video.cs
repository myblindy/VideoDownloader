namespace VideoDownloader.Models;

class Video : ReactiveObject
{
    public Video(Uri uri) => Uri = uri;

    public Uri Uri { get; }

    string name = null!;
    public string Name { get => name; set => this.RaiseAndSetIfChanged(ref name, value); }

    string downloadPath = null!;
    public string DownloadPath { get => downloadPath; set => this.RaiseAndSetIfChanged(ref downloadPath, value); }

    TimeSpan duration;
    public TimeSpan Duration { get => duration; set => this.RaiseAndSetIfChanged(ref duration, value); }

    long size;
    public long Size { get => size; set => this.RaiseAndSetIfChanged(ref size, value); }

    string format = null!;
    public string Format { get => format; set => this.RaiseAndSetIfChanged(ref format, value); }

    float completion;
    public float Completion { get => completion; set => this.RaiseAndSetIfChanged(ref completion, value); }

    float speedMBs;
    public float SpeedMBs { get => speedMBs; set => this.RaiseAndSetIfChanged(ref speedMBs, value); }

    TimeSpan eta;
    public TimeSpan ETA { get => eta; set => this.RaiseAndSetIfChanged(ref eta, value); }

    DownloadState state;
    public DownloadState State { get => state; set => this.RaiseAndSetIfChanged(ref state, value); }
}