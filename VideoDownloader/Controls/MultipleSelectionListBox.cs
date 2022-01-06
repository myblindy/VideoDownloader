using System.Windows.Controls;

namespace VideoDownloader.Controls;

class MultipleSelectionListBox : ListBox
{
    public static readonly DependencyProperty BindableSelectedItemsProperty =
        DependencyProperty.Register("BindableSelectedItems",
            typeof(IEnumerable<object>), typeof(MultipleSelectionListBox),
            new FrameworkPropertyMetadata(default(IEnumerable<object>),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBindableSelectedItemsChanged));

    public IEnumerable<object> BindableSelectedItems
    {
        get => (IEnumerable<object>)GetValue(BindableSelectedItemsProperty);
        set => SetValue(BindableSelectedItemsProperty, value);
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        BindableSelectedItems = SelectedItems.Cast<object>();
    }

    private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MultipleSelectionListBox listBox)
            listBox.SetSelectedItems(listBox.BindableSelectedItems);
    }
}