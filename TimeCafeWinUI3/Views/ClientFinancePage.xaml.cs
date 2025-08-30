using Microsoft.UI.Xaml.Controls;

namespace TimeCafeWinUI3.UI.Views;

public sealed partial class ClientFinancePage : Page
{
    public ClientFinanceViewModel ViewModel { get; }

    public ClientFinancePage()
    {
        ViewModel = App.GetService<ClientFinanceViewModel>();
        this.InitializeComponent();
    }
}