namespace VideoDownloader.ViewModels;

class MainViewModel : ReactiveObject
{
    public MainViewModel(IDialogService dialogService)
    {
        PasteCommand = ReactiveCommand.Create(async () =>
        {
            var uriString = Clipboard.GetText(TextDataFormat.Text);
            if (string.IsNullOrEmpty(uriString) || !Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri))
                dialogService.ShowMessageBox(this, $"Invalid text in clipboard:\n\n{uriString}", icon: MessageBoxImage.Error);
            else
            {
                var newVM = new AddNewViewModel(uri, dialogService);
                if (dialogService.ShowDialog(this, newVM) == true)
                {
                    Videos.Insert(0, newVM.Video);

                    var progress = new Progress<DownloadProgress>(dp =>
                    {
                        // ignore warnings
                        if (dp.State is DownloadState.Error && dp.Data.StartsWith("WARNING"))
                            return;

                        var (progress, state, speedString, etaString) = (dp.Progress, dp.State, dp.DownloadSpeed, dp.ETA);
                        var speedMBs = speedString is null || Regex.Match(speedString, @"^(\d+(?:\.\d+))([KMG])iB/s") is not { Success: true } match ? 0
                            : float.Parse(match.Groups[1].Value) * match.Groups[2].Value switch
                            {
                                "K" => 1.0f / 1024,
                                "M" => 1,
                                "G" => 1024,
                                _ => 0
                            };
                        if (etaString?.Count(c => c == ':') == 1)
                            etaString = $"0:{etaString}";
                        if (etaString is null || !TimeSpan.TryParse(etaString, out var eta))
                            eta = TimeSpan.Zero;

                        Application.Current.Dispatcher.BeginInvoke(() =>
                            (newVM.Video.Completion, newVM.Video.SpeedMBs, newVM.Video.ETA, newVM.Video.State) =
                                (progress, speedMBs, eta, state));
                    });

                    await App.YoutubeDL.RunVideoDownload(newVM.Video.Uri.ToString(),
                        newVM.SelectedFormat!.AudioBitrate > 0 ? newVM.SelectedFormat!.FormatId : $"{newVM.SelectedFormat!.FormatId}+bestaudio", progress: progress,
                        overrideOptions: new()
                        {
                            Output = newVM.Video.DownloadPath,
                            EmbedSubs = true,
                            SubLangs = newVM.SelectedSubtitles is null ? null : string.Join(',', newVM.SelectedSubtitles),
                        });
                }
            }
        });
    }

    public ObservableCollection<Video> Videos { get; } = new();
    public ICommand PasteCommand { get; }
}
