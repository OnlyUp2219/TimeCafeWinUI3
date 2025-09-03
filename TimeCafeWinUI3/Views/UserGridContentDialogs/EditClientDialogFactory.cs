using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TimeCafeWinUI3.UI.Views.UserGridContentDialogs;

public static class EditClientDialogFactory
{
    public static ContentDialog Create<T>(T data, XamlRoot xamlRoot, string title, string primaryButtonText = "Сохранить", string secondaryButtonText = "Отмена")
    {
        var dialog = new ContentDialog
        {
            Title = title,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = secondaryButtonText,
            DefaultButton = ContentDialogButton.Primary,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
            XamlRoot = xamlRoot
        };

        var editPage = new EditClientContentDialog();
        editPage.SetData(data);

        dialog.Content = editPage;
        dialog.PrimaryButtonClick += editPage.PrimaryButtonClick;

        return dialog;
    }
}