using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using TimeCafeWinUI3.Contracts.Services;


namespace TimeCafeWinUI3.ViewModels;

public partial class CreateClientViewModel : ObservableRecipient, INavigationAware
{
    private bool _isGridViewSelected = true;
    private int _currentPage = 1;
    private const int PageSize = 16;

    [ObservableProperty] private int totalItems;
    [ObservableProperty] private bool isLoading;

    public bool IsGridViewSelected
    {
        get => _isGridViewSelected;
        set
        {
            if (_isGridViewSelected != value)
            {
                _isGridViewSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ActiveView));

                // Синхронизируем выделение при переключении представлений
                if (AdaptiveGrid != null && ListView != null)
                {
                    if (_isGridViewSelected)
                    {
                        // Переключаемся на GridView, копируем выделение из ListView
                        var selectedItems = ListView.SelectedItems.ToList();
                        AdaptiveGrid.SelectedItems.Clear();
                        foreach (var item in selectedItems)
                        {
                            AdaptiveGrid.SelectedItems.Add(item);
                        }
                    }
                    else
                    {
                        // Переключаемся на ListView, копируем выделение из GridView
                        var selectedItems = AdaptiveGrid.SelectedItems.ToList();
                        ListView.SelectedItems.Clear();
                        foreach (var item in selectedItems)
                        {
                            ListView.SelectedItems.Add(item);
                        }
                    }
                }
            }
        }
    }

    public ListView ListView { get; set; }
    public AdaptiveGridView AdaptiveGrid { get; set; }

    public ListViewBase ActiveView => IsGridViewSelected ? AdaptiveGrid : ListView;

    [ObservableProperty] private ObservableCollection<Client> source = new();
    [ObservableProperty] private string firstName;
    [ObservableProperty] private string lastName;
    [ObservableProperty] private string middleName;
    [ObservableProperty] private int? genderId;
    [ObservableProperty] private string email;
    [ObservableProperty] private DateOnly? birthDate = DateOnly.FromDateTime(DateTime.Now);
    [ObservableProperty] private string phoneNumber;
    [ObservableProperty] private string accessCardNumber;
    [ObservableProperty] private string additionalInfo;
    [ObservableProperty] private string errorMessage;
    [ObservableProperty] private ObservableCollection<Gender> genders = new();
    [ObservableProperty] private ObservableCollection<ClientStatus> clientStatuses = new();

    private readonly IClientService _clientService;
    private readonly INavigationService _navigationService;
    private readonly FakeDataGenerator _fakeDataGenerator;

    public CreateClientViewModel(INavigationService navigationService, IClientService clientService)
    {
        _navigationService = navigationService;
        _clientService = clientService;
        _fakeDataGenerator = new FakeDataGenerator();
    }

    public async void OnNavigatedTo(object parameter)
    {
        await LoadDataAsync();
    }

    public void OnNavigatedFrom()
    {
        ClearData();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Source.Clear();
            Genders.Clear();
            ClientStatuses.Clear();

            // Загружаем первую страницу
            var (items, total) = await _clientService.GetClientsPageAsync(_currentPage, PageSize);
            TotalItems = total;
            
            foreach (var client in items)
            {
                Source.Add(client);
            }

            // Загружаем справочники
            var genders = await _clientService.GetGendersAsync();
            foreach (var gender in genders)
            {
                Genders.Add(gender);
            }

            var statuses = await _clientService.GetClientStatusesAsync();
            foreach (var status in statuses)
            {
                ClientStatuses.Add(status);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ClearData()
    {
        Source.Clear();
        Genders.Clear();
        ClientStatuses.Clear();
        TotalItems = 0;
        _currentPage = 1;
        
        FirstName = string.Empty;
        LastName = string.Empty;
        MiddleName = string.Empty;
        GenderId = null;
        Email = string.Empty;
        BirthDate = null;
        PhoneNumber = string.Empty;
        AccessCardNumber = string.Empty;
        AdditionalInfo = string.Empty;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task CreateClientAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "Имя и фамилия обязательны для заполнения";
            return;
        }

        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ErrorMessage = "Номер телефона обязателен для заполнения";
            return;
        }

        var isValidPhone = await _clientService.ValidatePhoneNumberAsync(PhoneNumber);
        if (!isValidPhone)
        {
            ErrorMessage = "Неверный формат номера телефона или номер уже существует";
            return;
        }

        if (!string.IsNullOrWhiteSpace(Email))
        {
            var isValidEmail = await _clientService.ValidateEmailAsync(Email);
            if (!isValidEmail)
            {
                ErrorMessage = "Неверный формат email или email уже существует";
                return;
            }
        }

        var client = new Client
        {
            FirstName = FirstName,
            LastName = LastName,
            MiddleName = MiddleName,
            GenderId = GenderId,
            Email = Email,
            BirthDate = BirthDate,
            PhoneNumber = PhoneNumber,
            AccessCardNumber = AccessCardNumber,
            CreatedAt = DateTime.Now
        };

        if (!string.IsNullOrWhiteSpace(AdditionalInfo))
        {
            client.ClientAdditionalInfos.Add(new ClientAdditionalInfo
            {
                InfoText = AdditionalInfo,
                CreatedAt = DateTime.Now
            });
        }

        try
        {
            await _clientService.CreateClientAsync(client);
            await LoadDataAsync(); // Перезагружаем первую страницу
            ClearData();
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при создании клиента: {ex.Message}";
        }
    }

    [RelayCommand]
    private void GenerateFakeData()
    {
        var fakeClient = _fakeDataGenerator.GenerateClient();

        FirstName = fakeClient.FirstName;
        LastName = fakeClient.LastName;
        MiddleName = fakeClient.MiddleName;
        GenderId = fakeClient.GenderId;
        Email = fakeClient.Email;
        BirthDate = fakeClient.BirthDate;
        PhoneNumber = fakeClient.PhoneNumber;
        AccessCardNumber = fakeClient.AccessCardNumber;
    }

    public async Task SetCurrentPage(int page)
    {
        if (_currentPage != page)
        {
            try
            {
                IsLoading = true;
                _currentPage = page;
                Source.Clear();

                var (items, total) = await _clientService.GetClientsPageAsync(_currentPage, PageSize);
                TotalItems = total;

                foreach (var client in items)
                {
                    Source.Add(client);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
