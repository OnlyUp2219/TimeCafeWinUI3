using CommunityToolkit.Mvvm.ComponentModel;
using TimeCafeWinUI3.Core.Models;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.Views;

namespace TimeCafeWinUI3.ViewModels;

public partial class UserGridDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly IClientService _clientService;

    [ObservableProperty]
    private Client? _item;


    public bool HasAdditionalInfo => Item?.ClientAdditionalInfos?.Any() ?? false;

    public UserGridDetailViewModel(IClientService clientService)
    {
        _clientService = clientService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is Client client)
        {
            Item = client;
        }
        else if (parameter is string phoneNumber)
        {
            var clients = await _clientService.GetAllClientsAsync();
            Item = clients.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
        }
    }

    public void OnNavigatedFrom()
    {
        Item = null;
    }

    [RelayCommand]
    private async Task RefuseServiceAsync()
    {
        var refuseService = App.GetService<RefuseServiceContentDialog>();

        var dialog = new ContentDialog
        {
            XamlRoot = App.MainWindow.Content.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
            Title = "Отказ от услуг",
            PrimaryButtonText = "Подтвердить",
            SecondaryButtonText = "Отмена",
            CloseButtonText = "Отмена",
            DefaultButton = ContentDialogButton.Primary,
            Content = refuseService
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {

        }
    }

    [RelayCommand]
    private async Task EditClientAsync()
    {
        if (Item == null) return;

        var dialog = EditClientDialogFactory.Create(
            Item,
            App.MainWindow.Content.XamlRoot,
            "Редактирование клиента"
        );

        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.RequestedTheme = App.GetService<IThemeSelectorService>().Theme;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var editClient = (EditClientContentDialog)((ContentDialog)dialog).Content;
            var updatedClient = editClient.ViewModel.GetUpdatedClient();
            await _clientService.UpdateClientAsync(updatedClient);
            
            if (editClient.ViewModel.IsPhoneNumberChanged())
            {
                var isPhoneVerified = await VerifyPhoneNumberAsync(updatedClient.PhoneNumber);
                if (isPhoneVerified)
                {
                    await _clientService.SetClientActiveAsync(updatedClient.ClientId);
                }
                else
                {
                    await _clientService.SetClientDraftAsync(updatedClient.ClientId);
                }
            }


            Item = null; 
            OnPropertyChanged(nameof(Item)); 
            Item = await _clientService.GetClientByIdAsync(updatedClient.ClientId);
            OnPropertyChanged(nameof(Item));
            OnPropertyChanged(nameof(HasAdditionalInfo));
        }
    }

    private async Task<bool> VerifyPhoneNumberAsync(string phoneNumber)
    {
        while (true)
        {
            var dialog = PhoneVerificationDialogFactory.Create(
                phoneNumber,
                App.MainWindow.Content.XamlRoot
            );

            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.RequestedTheme = App.GetService<IThemeSelectorService>().Theme;

            // TODO: Реализовать отправку SMS
            await _clientService.SendPhoneConfirmationCodeAsync(phoneNumber);

            var verificationResult = await dialog.ShowAsync();

            if (verificationResult == ContentDialogResult.Secondary)
            {
                return false;
            }

            if (verificationResult == ContentDialogResult.Primary)
            {
                var phoneVerification = (PhoneVerificationConfirm)dialog.Content;
                var code = phoneVerification.VerificationCodeInput.Text;
                if (code == "12345") // TODO: Заменить на реальную проверку кода
                {
                    return true;
                }

                var errorDialog = new ContentDialog
                {
                    XamlRoot = App.MainWindow.Content.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
                    Title = "Ошибка",
                    Content = "Неверный код подтверждения",
                    CloseButtonText = "OK"
                };
                await errorDialog.ShowAsync();
                continue;
            }

            return false;
        }
    }
}
