using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.ViewModels;

namespace TimeCafeWinUI3.Views;

public sealed partial class ClientFinancePage : Page
{
    public ClientFinanceViewModel ViewModel { get; }

    public ClientFinancePage()
    {
        ViewModel = App.GetService<ClientFinanceViewModel>();
        this.InitializeComponent();
    }
} 