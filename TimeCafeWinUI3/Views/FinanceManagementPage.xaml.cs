using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.ViewModels;

namespace TimeCafeWinUI3.Views;

public sealed partial class FinanceManagementPage : Page
{
    public FinanceManagementViewModel ViewModel { get; }

    public FinanceManagementPage()
    {
        ViewModel = App.GetService<FinanceManagementViewModel>();
        this.InitializeComponent();
    }
} 