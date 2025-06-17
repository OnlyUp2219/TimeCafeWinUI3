using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.ViewModels;

namespace TimeCafeWinUI3.Views;

public sealed partial class VisitorManagementPage : Page
{
    public VisitorManagementViewModel ViewModel
    {
        get;
    }

    public VisitorManagementPage()
    {
        ViewModel = App.GetService<VisitorManagementViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
} 