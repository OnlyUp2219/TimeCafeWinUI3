namespace TimeCafeWinUI3.ViewModels;

public partial class UserGridDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly IClientService _clientService;

    [ObservableProperty]
    private Client? item;

    public UserGridDetailViewModel(IClientService clientService)
    {
        _clientService = clientService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is string phoneNumber)
        {
            var clients = await _clientService.GetAllClientsAsync();
            Item = clients.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
        }
    }

    public void OnNavigatedFrom()
    {
        Item = null;
    }
}
