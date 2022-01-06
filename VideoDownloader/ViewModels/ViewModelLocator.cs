namespace VideoDownloader.ViewModels;

class ViewModelLocator
{
    public ViewModelLocator()
    {
        SimpleIoc.Default.Register<MainViewModel>();
    }

    public MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();
}
