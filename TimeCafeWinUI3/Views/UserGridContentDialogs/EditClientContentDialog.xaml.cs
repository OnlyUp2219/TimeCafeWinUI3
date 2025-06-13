using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.Core.Models;
using TimeCafeWinUI3.ViewModels;

namespace TimeCafeWinUI3.Views;

public sealed partial class EditClientContentDialog : Page
{
    public EditClientContentDialogViewModel ViewModel { get; }

    public EditClientContentDialog()
    {
        ViewModel = App.GetService<EditClientContentDialogViewModel>();
        DataContext = ViewModel;
        this.InitializeComponent();
    }

    public void SetClient(Client client)
    {
        ViewModel.SetClient(client);
    }

    [RelayCommand]
    private async void PrimaryButtonClick(ContentDialogButtonClickEventArgs args)
    {
        var validationResult = await ViewModel.ValidateAsync();
        if (!string.IsNullOrEmpty(validationResult))
        {
            ViewModel.ErrorMessage = validationResult;
            args.Cancel = true;
            return;
        }

        if (!ViewModel.Validate())
        {
            args.Cancel = true;
            return;
        }
    }
} 