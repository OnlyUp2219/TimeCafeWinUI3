using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TimeCafeWinUI3.Views;

public static class PhoneVerificationDialogFactory
{
    public static ContentDialog Create(string phoneNumber, XamlRoot xamlRoot)
    {
        var dialog = new ContentDialog
        {
            Title = "Подтверждение телефона",
            PrimaryButtonText = "Подтвердить",
            SecondaryButtonText = "Пропустить",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = xamlRoot
        };

        var phoneVerification = new PhoneVerificationConfirm();
        var phoneVerificationVM = App.GetService<PhoneVerificationViewModel>();
        phoneVerificationVM.SetPhoneNumber(phoneNumber);
        phoneVerification.DataContext = phoneVerificationVM;
        dialog.Content = phoneVerification;
        dialog.PrimaryButtonClick += phoneVerification.PrimaryButtonClick;

        return dialog;
    }
} 