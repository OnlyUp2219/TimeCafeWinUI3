using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.UI.Views.CreateClientPages;
using TimeCafeWinUI3.UI.Views.UserGridContentDialogs;

namespace TimeCafeWinUI3.UI.ViewModels;

public partial class UserGridDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly IClientQueries _clientQueries;
    private readonly IClientCommands _clientCommands;
    private readonly IClientAdditionalInfoQueries _additionalInfoQueries;
    private readonly IClientAdditionalInfoCommands _additionalInfoCommands;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Client? _item;


    public bool HasAdditionalInfo => Item?.ClientAdditionalInfos?.Any() ?? false;

    public UserGridDetailViewModel(IClientQueries clientQueries,
        IClientAdditionalInfoQueries additionalInfoQueries,
        INavigationService navigationService,
        IClientAdditionalInfoCommands additionalInfoCommands,
        IClientCommands clientCommands)
    {
        _clientQueries = clientQueries;
        _additionalInfoQueries = additionalInfoQueries;
        _navigationService = navigationService;
        _additionalInfoCommands = additionalInfoCommands;
        _clientCommands = clientCommands;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is Client client)
        {
            Item = client;
            if (client.ClientAdditionalInfos == null)
            {
                var additionalInfos = await _additionalInfoQueries.GetClientAdditionalInfosAsync(client.ClientId);
                client.ClientAdditionalInfos = additionalInfos.ToList();
                OnPropertyChanged(nameof(Item.ClientAdditionalInfos));
            }
        }
        else if (parameter is string phoneNumber)
        {
            var clients = await _clientQueries.GetAllClientsAsync();
            Item = clients.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
            if (Item != null)
            {
                var additionalInfos = await _additionalInfoQueries.GetClientAdditionalInfosAsync(Item.ClientId);
                Item.ClientAdditionalInfos = additionalInfos.ToList();
                OnPropertyChanged(nameof(Item.ClientAdditionalInfos));
            }
        }
        else if (parameter is int clientId)
        {
            var clients = await _clientQueries.GetAllClientsAsync();
            Item = clients.FirstOrDefault(c => c.ClientId == clientId);
            if (Item != null)
            {
                var additionalInfos = await _additionalInfoQueries.GetClientAdditionalInfosAsync(Item.ClientId);
                Item.ClientAdditionalInfos = additionalInfos.ToList();
                OnPropertyChanged(nameof(Item.ClientAdditionalInfos));
            }
        }
    }

    public void OnNavigatedFrom()
    {
        Item = null;
    }

    [RelayCommand]
    private async Task RefuseServiceAsync()
    {
        if (Item == null) return;

        var dialog = RefuseServiceDialogFactory.Create(
            Item,
            App.MainWindow.Content.XamlRoot
        );

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var refuseService = (RefuseServiceContentDialog)dialog.Content;
            var reason = refuseService.ViewModel.Reason;

            var additionalInfo = new ClientAdditionalInfo
            {
                ClientId = Item.ClientId,
                InfoText = $"Отказ от услуг. \nПричина: {reason}",
                CreatedAt = DateTime.Now
            };

            await _additionalInfoCommands.CreateAdditionalInfoAsync(additionalInfo);
            await _clientCommands.SetClientRejectedAsync(Item.ClientId, reason);

            // Обновляем клиента и его дополнительную информацию
            Item = await _clientQueries.GetClientByIdAsync(Item.ClientId);
            var additionalInfos = await _additionalInfoQueries.GetClientAdditionalInfosAsync(Item.ClientId);
            Item.ClientAdditionalInfos = additionalInfos.ToList();

            OnPropertyChanged(nameof(Item));
            OnPropertyChanged(nameof(HasAdditionalInfo));
            OnPropertyChanged(nameof(Item.ClientAdditionalInfos));
        }
    }

    [RelayCommand]
    private async Task VerifyPhoneAsync()
    {
        if (Item == null) return;

        var isPhoneVerified = await VerifyPhoneNumberAsync(Item.PhoneNumber);
        if (isPhoneVerified)
        {
            await _clientCommands.SetClientActiveAsync(Item.ClientId);
            Item = await _clientQueries.GetClientByIdAsync(Item.ClientId);
            OnPropertyChanged(nameof(Item));
            OnPropertyChanged(nameof(HasAdditionalInfo));
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

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var editClient = (EditClientContentDialog)((ContentDialog)dialog).Content;
            var updatedClient = editClient.ViewModel.GetUpdatedClient();
            await _clientCommands.UpdateClientAsync(updatedClient);

            if (editClient.ViewModel.IsPhoneNumberChanged())
            {
                var isPhoneVerified = await VerifyPhoneNumberAsync(updatedClient.PhoneNumber);
                if (isPhoneVerified)
                {
                    await _clientCommands.SetClientActiveAsync(updatedClient.ClientId);
                }
                else
                {
                    await _clientCommands.SetClientDraftAsync(updatedClient.ClientId);
                }
            }


            Item = null;
            OnPropertyChanged(nameof(Item));
            Item = await _clientQueries.GetClientByIdAsync(updatedClient.ClientId);
            OnPropertyChanged(nameof(Item));
            OnPropertyChanged(nameof(HasAdditionalInfo));
        }
    }

    private async Task<bool> VerifyPhoneNumberAsync(string phoneNumber)
    {
        var dialog = PhoneVerificationDialogFactory.Create(
            phoneNumber,
            App.MainWindow.Content.XamlRoot
        );

        // TODO: Реализовать отправку SMS
        //await _clientService.SendPhoneConfirmationCodeAsync(phoneNumber);

        var verificationResult = await dialog.ShowAsync();

        if (verificationResult == ContentDialogResult.Secondary)
        {
            return false;
        }

        if (verificationResult == ContentDialogResult.Primary)
        {
            //var phoneVerification = (PhoneVerificationConfirm)dialog.Content;
            //var code = phoneVerification.VerificationCodeInput.Text;
            //return await _clientService.VerifyPhoneConfirmationCodeAsync(phoneNumber, code);
            return true;
        }

        return false;
    }

    [RelayCommand]
    private async Task DeleteClientAsync()
    {
        var dialog = new ContentDialog
        {
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
            XamlRoot = App.MainWindow.Content.XamlRoot,
            Title = "Подтверждение",
            Content = "Вы уверены, что хотите удалить этого клиента?",
            PrimaryButtonText = "Удалить",
            CloseButtonText = "Отмена",
            DefaultButton = ContentDialogButton.Close
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary && Item != null)
        {
            await _clientCommands.DeleteClientAsync(Item.ClientId);
            _navigationService.GoBack();
        }
    }

    [RelayCommand]
    private void Finance()
    {
        if (Item != null)
        {
            _navigationService.NavigateTo(typeof(ClientFinanceViewModel).FullName, Item);
        }
    }

    [RelayCommand]
    private async Task AddNoteAsync()
    {
        if (Item == null) return;

        var dialog = RefuseServiceDialogFactory.Create(
            Item,
            App.MainWindow.Content.XamlRoot
        );

        dialog.Title = "Добавить заметку";
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var addNoteDialog = (RefuseServiceContentDialog)dialog.Content;
            var noteText = addNoteDialog.ViewModel.Reason;

            var additionalInfo = new ClientAdditionalInfo
            {
                ClientId = Item.ClientId,
                InfoText = noteText,
                CreatedAt = DateTime.Now
            };

            await _additionalInfoCommands.CreateAdditionalInfoAsync(additionalInfo);

            var additionalInfos = await _additionalInfoQueries.GetClientAdditionalInfosAsync(Item.ClientId);
            Item.ClientAdditionalInfos = additionalInfos.ToList();

            OnPropertyChanged(nameof(Item));
            OnPropertyChanged(nameof(HasAdditionalInfo));
            OnPropertyChanged(nameof(Item.ClientAdditionalInfos));
        }
    }
}
