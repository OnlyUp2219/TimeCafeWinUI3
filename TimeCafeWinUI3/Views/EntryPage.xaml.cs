using Microsoft.UI.Xaml.Controls;

namespace TimeCafeWinUI3.Views;

public sealed partial class EntryPage : Page
{
    public EntryViewModel ViewModel
    {
        get;
    }

    public EntryPage()
    {
        ViewModel = App.GetService<EntryViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public void ShowError(string message)
    {
        ErrorTeachingTip.Subtitle = message;
        ErrorTeachingTip.IsOpen = true;
    }
}