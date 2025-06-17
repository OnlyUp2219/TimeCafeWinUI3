using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.ViewModels;

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
} 