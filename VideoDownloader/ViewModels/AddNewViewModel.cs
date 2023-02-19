using System.IO;
using System.Linq;
using VideoDownloader.Properties;

namespace VideoDownloader.ViewModels;

class AddNewViewModel : ReactiveObject, IModalDialogViewModel
{
    public AddNewViewModel(Uri uri, IDialogService dialogService)
    {
        Video = new(uri);
        cancellationTokenSource = new();

        Task.Run(async () =>
        {
            var data = await App.YoutubeDL.RunVideoDataFetch(uri.ToString(), cancellationTokenSource.Token);

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                Success = data.Success && data.Data.Duration is not null;
                if (Success == true)
                {
                    Video.Name = data.Data.Title;
                    Video.Duration = TimeSpan.FromSeconds((double)data.Data.Duration!);
                    if (data.Data.Subtitles is not null)
                        Subtitles.AddRange(data.Data.Subtitles.Keys.OrderBy(w => w));

                    var formatsRoot = data.Data.Formats.AsEnumerable();
                    if (formatsRoot.Any() && formatsRoot.Any(f => f.FrameRate is not null))
                        formatsRoot = formatsRoot.Where(f => f.FrameRate is not null);
                    Formats.AddRange(formatsRoot
                        .Where(f => f.FileSize is not null)
                        .OrderByDescending(f => f.FrameRate)
                            .ThenByDescending(f => f.Width * f.Height)
                            .ThenByDescending(f => f.ContainerFormat));
                    SelectedFormat = Formats.FirstOrDefault();
                    SelectedSubtitles = Subtitles.Where(s => Regex.IsMatch(s, @"^en(?:\b|[_])"));

                    var fn = Regex.Replace(Video.Name, $@"({string.Join('|', Path.GetInvalidFileNameChars().Select(c => Regex.Escape(c.ToString())))})+", " ")
                        + "." + data.Data.Extension;
                    Video.DownloadPath = string.IsNullOrWhiteSpace(Settings.Default.LastOutputFolder) ? fn : Path.Combine(Settings.Default.LastOutputFolder, fn);
                }
            });
        }, cancellationTokenSource.Token);

        BrowseOutputCommand = ReactiveCommand.Create(() =>
        {
            var ext = Path.GetExtension(Video.DownloadPath);
            MvvmDialogs.FrameworkDialogs.SaveFile.SaveFileDialogSettings settings = new()
            {
                FileName = Video.DownloadPath,
                Filter = $"{ext} Files|*{ext}|All Files|*.*",
            };
            if (dialogService.ShowSaveFileDialog(this, settings) == true)
                Video.DownloadPath = settings.FileName;
        }, this.WhenAnyValue(x => x.Success).Select(s => s == true));

        DownloadCommand = ReactiveCommand.Create(() =>
        {
            Settings.Default.LastOutputFolder = string.IsNullOrWhiteSpace(Video.DownloadPath) ? null : Path.GetDirectoryName(Video.DownloadPath);
            Settings.Default.Save();

            Video.Size = SelectedFormat!.FileSize!.Value;
            DialogResult = true;
        }, this.WhenAnyValue(x => x.Success).Select(s => s == true));

        CancelCommand = ReactiveCommand.Create(() => DialogResult = false);
    }

    bool? dialogResult;
    public bool? DialogResult { get => dialogResult; set => this.RaiseAndSetIfChanged(ref dialogResult, value); }

    public Video Video { get; }
    readonly CancellationTokenSource cancellationTokenSource;

    bool? success;
    public bool? Success { get => success; set => this.RaiseAndSetIfChanged(ref success, value); }

    public ObservableCollection<string> Subtitles { get; } = new();
    public ObservableCollection<FormatData> Formats { get; } = new();

    FormatData? selectedFormat;
    public FormatData? SelectedFormat { get => selectedFormat; set => this.RaiseAndSetIfChanged(ref selectedFormat, value); }

    IEnumerable<string>? selectedSubtitles;
    public IEnumerable<string>? SelectedSubtitles { get => selectedSubtitles; set => this.RaiseAndSetIfChanged(ref selectedSubtitles, value); }

    public ICommand BrowseOutputCommand { get; }
    public ICommand DownloadCommand { get; }
    public ICommand CancelCommand { get; }
}
