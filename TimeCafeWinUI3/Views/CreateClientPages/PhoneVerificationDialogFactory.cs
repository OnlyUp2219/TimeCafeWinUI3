using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TimeCafeWinUI3.Views;

public static class PhoneVerificationDialogFactory
{
    public static ContentDialog Create<T>(T data, XamlRoot xamlRoot, string title = "Подтверждение телефона", string primaryButtonText = "Подтвердить", string secondaryButtonText = "Пропустить", string closeButtonText = "Отменить")
    {
        var dialog = new ContentDialog
        {
            Title = title,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = secondaryButtonText,
            CloseButtonText = closeButtonText,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = xamlRoot
        };

        var phoneVerification = new PhoneVerificationConfirm();


        if (data is string phoneNumber)
        {
            phoneVerification.SetPhoneNumber(phoneNumber);
        }
        if (data is Client client)
        {
            phoneVerification.SetPhoneNumber(client.PhoneNumber);
        }

        dialog.Content = phoneVerification;
        dialog.PrimaryButtonClick += phoneVerification.PrimaryButtonClick;

        return dialog;
    }
}