using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TimeCafeWinUI3.Views;

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
            XamlRoot = xamlRoot
        };

        var editPage = new EditClientContentDialog();
        editPage.SetData(data);
        dialog.Content = editPage;

        dialog.PrimaryButtonClick += editPage.PrimaryButtonClick;

        return dialog;
    }
} 