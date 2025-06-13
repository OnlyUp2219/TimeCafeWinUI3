using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TimeCafeWinUI3.Views;

public static class RefuseServiceDialogFactory
{
    public static ContentDialog Create<T>(T data, XamlRoot xamlRoot, string title = "Отказ от услуг", string primaryButtonText = "Подтвердить", string secondaryButtonText = "Отмена", string closeButtonText = "Отмена")
    {
        var dialog = new ContentDialog
        {
            Title = title,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = secondaryButtonText,
            CloseButtonText = closeButtonText,
            DefaultButton = ContentDialogButton.Primary,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
            XamlRoot = xamlRoot
        };

        var refuseService = new RefuseServiceContentDialog();
        if (data is Client client)
        {
            refuseService.ViewModel.SetClient(client);
        }

        dialog.Content = refuseService;
        dialog.PrimaryButtonClick += refuseService.PrimaryButtonClick;

        return dialog;
    }
} 