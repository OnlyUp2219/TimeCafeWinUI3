using Microsoft.UI.Xaml.Controls;

namespace TimeCafeWinUI3.Views;

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
