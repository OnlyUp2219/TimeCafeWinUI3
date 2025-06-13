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

    public async void PrimaryButtonClick(object sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        ViewModel.ErrorMessage = string.Empty;

        var validationResult = await ViewModel.ValidConfirmCode();
        if (!string.IsNullOrEmpty(validationResult))
        {
            ViewModel.ErrorMessage = validationResult;
            args.Cancel = true;
        }
        else
        {
            args.Cancel = false;
        }

        deferral.Complete();
    }
}
