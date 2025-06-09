using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.Core.Contracts.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimeCafeWinUI3.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PhoneVerificationConfirm : Page
{
    private readonly IClientService _clientService;
    private string _phoneNumber;
    private string _verificationCode;

    public PhoneVerificationViewModel ViewModel
    {
        get;
    }

    public PhoneVerificationConfirm()
    {
        ViewModel = App.GetService<PhoneVerificationViewModel>();
        _clientService = App.GetService<IClientService>();
        InitializeComponent();
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        PhoneNumberText.Text = phoneNumber;
    }

    public void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
    }

    public void ShowCodeSent()
    {
        ErrorText.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        CodeInfoText.Text = "Код подтверждения отправлен";
    }
}
