using CommunityToolkit.Mvvm.ComponentModel;
using TimeCafeWinUI3.Core.Models;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

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
        var editClient = App.GetService<EditClientContentDialog>();
        editClient.SetClient(Item);

        var dialog = new ContentDialog
        {
            XamlRoot = App.MainWindow.Content.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
            Title = "Редактирование клиента",
            PrimaryButtonText = "Сохранить",
            CloseButtonText = "Отмена",
            DefaultButton = ContentDialogButton.Primary,
            Content = editClient
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
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

            // Сбрасываем Item и уведомляем UI
            Item = null; // Очищаем, чтобы сбросить все привязки
            OnPropertyChanged(nameof(Item)); // Уведомляем UI
            Item = await _clientService.GetClientByIdAsync(updatedClient.ClientId);
            OnPropertyChanged(nameof(Item)); // Уведомляем UI снова
            OnPropertyChanged(nameof(HasAdditionalInfo)); // Обновляем вычисляемое свойство
        }
    }

    private async Task<bool> VerifyPhoneNumberAsync(string phoneNumber)
    {
        while (true)
        {
            var phoneVerification = new PhoneVerificationConfirm();
            var phoneVerificationVM = App.GetService<PhoneVerificationViewModel>();
            phoneVerificationVM.SetPhoneNumber(phoneNumber);
            phoneVerification.DataContext = phoneVerificationVM;

            var verificationDialog = new ContentDialog
            {
                XamlRoot = App.MainWindow.Content.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
                Title = "Подтверждение телефона",
                PrimaryButtonText = "Подтвердить",
                SecondaryButtonText = "Пропустить",
                CloseButtonText = "Отмена",
                DefaultButton = ContentDialogButton.Primary,
                Content = phoneVerification
            };

            // TODO: Реализовать отправку SMS
            await _clientService.SendPhoneConfirmationCodeAsync(phoneNumber);

            var verificationResult = await verificationDialog.ShowAsync();

            if (verificationResult == ContentDialogResult.Secondary)
            {
                return false;
            }

            if (verificationResult == ContentDialogResult.Primary)
            {
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
