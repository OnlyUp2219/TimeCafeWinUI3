using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;
using System.Text;

namespace TimeCafeWinUI3.ViewModels;

public enum EntryState
{
    Welcome,
    CardInput,
    Registration,
    TariffSelection,
    Success
}

public partial class EntryViewModel : ObservableRecipient, INavigationAware
{
    private readonly IClientService _clientService;
    private readonly ITariffService _tariffService;
    private readonly IVisitService _visitService;
    private readonly IWorkingHoursService _workingHoursService;
    private readonly INavigationService _navigationService;
    private readonly DispatcherTimer _countdownTimer;

    [ObservableProperty] private EntryState currentState = EntryState.Welcome;
    [ObservableProperty] private string cardNumber;
    [ObservableProperty] private string errorMessage;
    [ObservableProperty] private int countdownSeconds = 15;
    [ObservableProperty] private Client currentClient;
    [ObservableProperty] private Tariff selectedTariff;
    [ObservableProperty] private ObservableCollection<Tariff> tariffs = new();
    [ObservableProperty] private ObservableCollection<Gender> genders = new();

    // Registration properties
    [ObservableProperty] private string firstName;
    [ObservableProperty] private string lastName;
    [ObservableProperty] private string middleName;
    [ObservableProperty] private int? genderId;
    [ObservableProperty] private string email;
    [ObservableProperty] private DateOnly? birthDate = DateOnly.FromDateTime(DateTime.Now);
    [ObservableProperty] private string phoneNumber;
    [ObservableProperty] private string additionalInfo;

    // Track the path user took
    private bool _isRegistrationPath = false;

    public EntryViewModel(INavigationService navigationService, IClientService clientService, ITariffService tariffService, IVisitService visitService, IWorkingHoursService workingHoursService)
    {
        _navigationService = navigationService;
        _clientService = clientService;
        _tariffService = tariffService;
        _visitService = visitService;
        _workingHoursService = workingHoursService;

        _countdownTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _countdownTimer.Tick += CountdownTimer_Tick;
    }

    public async void OnNavigatedTo(object parameter)
    {
        await LoadDataAsync();
    }

    public void OnNavigatedFrom()
    {
        _countdownTimer.Stop();
        ClearData();
    }
    private async Task LoadDataAsync()
    {
        try
        {
            var tariffs = await _tariffService.GetAllTariffsAsync();
            Tariffs.Clear();
            foreach (var tariff in tariffs.Take(10)) 
            {
                Tariffs.Add(tariff);
            }

            var genders = await _clientService.GetGendersAsync();
            Genders.Clear();
            foreach (var gender in genders)
            {
                Genders.Add(gender);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки данных: {ex.Message}";
        }
    }

    private void ClearData()
    {
        CardNumber = string.Empty;
        ErrorMessage = string.Empty;
        CurrentClient = null;
        SelectedTariff = null;
        FirstName = string.Empty;
        LastName = string.Empty;
        MiddleName = string.Empty;
        GenderId = null;
        Email = string.Empty;
        BirthDate = DateOnly.FromDateTime(DateTime.Now);
        PhoneNumber = string.Empty;
        AdditionalInfo = string.Empty;
        CountdownSeconds = 15;
        _isRegistrationPath = false;
    }

    public bool IsWelcomeState => CurrentState == EntryState.Welcome;
    public bool IsCardInputState => CurrentState == EntryState.CardInput;
    public bool IsRegistrationState => CurrentState == EntryState.Registration;
    public bool IsTariffSelectionState => CurrentState == EntryState.TariffSelection;
    public bool IsSuccessState => CurrentState == EntryState.Success;
    public bool CanGoBack => CurrentState != EntryState.Welcome;
    public bool CanGoNext => CanProceedToNext();

    private bool CanProceedToNext()
    {
        return CurrentState switch
        {
            EntryState.Welcome => false, 
            EntryState.CardInput => !string.IsNullOrWhiteSpace(CardNumber),
            EntryState.Registration => IsRegistrationValidSync(),
            EntryState.TariffSelection => SelectedTariff != null,
            EntryState.Success => true, 
            _ => false
        };
    }

    private bool IsRegistrationValidSync()
    {
        return !string.IsNullOrWhiteSpace(FirstName) &&
               !string.IsNullOrWhiteSpace(LastName) &&
               !string.IsNullOrWhiteSpace(PhoneNumber);
    }

    private async Task<bool> IsRegistrationValid()
    {
        return !string.IsNullOrWhiteSpace(FirstName) &&
               !string.IsNullOrWhiteSpace(LastName) &&
               !string.IsNullOrWhiteSpace(PhoneNumber) &&
               await _clientService.ValidatePhoneNumberAsync(PhoneNumber);
    }

    [RelayCommand]
    private void EnterAsMember()
    {
        _isRegistrationPath = false;
        CurrentState = EntryState.CardInput;
        ErrorMessage = string.Empty;
        UpdateStateProperties();
    }

    [RelayCommand]
    private void Register()
    {
        _isRegistrationPath = true;
        CurrentState = EntryState.Registration;
        ErrorMessage = string.Empty;
        UpdateStateProperties();
    }

    [RelayCommand]
    private async Task Back()
    {
        switch (CurrentState)
        {
            case EntryState.CardInput:
            case EntryState.Registration:
                CurrentState = EntryState.Welcome;
                ClearData();
                break;
            case EntryState.TariffSelection:
                // Return to the correct previous state based on path
                if (_isRegistrationPath)
                {
                    CurrentState = EntryState.Registration;
                }
                else
                {
                    CurrentState = EntryState.CardInput;
                }
                break;
            case EntryState.Success:
                CurrentState = EntryState.Welcome;
                ClearData();
                break;
        }

        ErrorMessage = string.Empty;
        UpdateStateProperties();
    }

    [RelayCommand]
    private async Task Next()
    {
        switch (CurrentState)
        {
            case EntryState.CardInput:
                await ProcessCardInput();
                break;
            case EntryState.Registration:
                await ProcessRegistration();
                break;
            case EntryState.TariffSelection:
                await ProcessTariffSelection();
                break;
            case EntryState.Success:
                SkipWait();
                break;
        }
    }

    private async Task ProcessCardInput()
    {
        if (string.IsNullOrWhiteSpace(CardNumber))
        {
            await ShowErrorAsync("Введите номер карты СКУД");
            return;
        }

        try
        {
            // TODO: Implement card validation logic
            // For now, simulate finding a client
            var clients = await _clientService.GetAllClientsAsync();
            CurrentClient = clients.FirstOrDefault(c => c.AccessCardNumber == CardNumber);

            if (CurrentClient == null)
            {
                await ShowErrorAsync("Такого клиента нет или карты СКУД не правильная");
                return;
            }

            if (CurrentClient.StatusId != (int)ClientStatusType.Active)
            {
                await ShowErrorAsync("Клиент не в статусе активный");
                return;
            }

            CurrentState = EntryState.TariffSelection;
            ErrorMessage = string.Empty;
            UpdateStateProperties();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Ошибка поиска клиента: {ex.Message}");
        }
    }

    private async Task ProcessRegistration()
    {
        var validationResult = await ValidateRegistration();
        if (!string.IsNullOrEmpty(validationResult))
        {
            await ShowErrorAsync(validationResult);
            return;
        }

        try
        {
            // Show phone verification dialog
            var dialog = PhoneVerificationDialogFactory.Create(
                PhoneNumber,
                App.MainWindow.Content.XamlRoot,
                "Подтверждение телефона",
                "Подтвердить",
                "Пропустить",
                "Отменить"
            );

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await CreateClient(true);
                CurrentState = EntryState.TariffSelection;
            }
            else if (result == ContentDialogResult.Secondary)
            {
                await CreateClient(false);
                await ShowErrorAsync("Клиент не в статусе активный");
                return;
            }
            else
            {
                return;
            }

            ErrorMessage = string.Empty;
            UpdateStateProperties();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Ошибка регистрации: {ex.Message}");
        }
    }

    private async Task<string> ValidateRegistration()
    {
        var sb = new StringBuilder();

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            sb.AppendLine("Имя и фамилия обязательны для заполнения");

        if (string.IsNullOrWhiteSpace(PhoneNumber))
            sb.AppendLine("Номер телефона обязателен для заполнения");
        else
        {
            var validPhone = await _clientService.ValidatePhoneNumberAsync(PhoneNumber);
            if (!validPhone)
                sb.AppendLine("Неверный формат номера телефона или такой номер уже существует");
        }

        if (!string.IsNullOrWhiteSpace(Email))
        {
            var validMail = await _clientService.ValidateEmailAsync(Email);
            if (!validMail)
                sb.AppendLine("Неверный формат email");
        }

        return sb.ToString();
    }

    private async Task CreateClient(bool isActive)
    {
        var client = new Client
        {
            FirstName = FirstName,
            LastName = LastName,
            MiddleName = MiddleName,
            GenderId = GenderId,
            Email = Email,
            BirthDate = BirthDate,
            PhoneNumber = PhoneNumber,
            StatusId = isActive ? (int)ClientStatusType.Active : (int)ClientStatusType.Draft,
            CreatedAt = DateTime.Now
        };

        if (isActive)
        {
            client.AccessCardNumber = await _clientService.GenerateAccessCardNumberAsync();
        }

        CurrentClient = await _clientService.CreateClientAsync(client);
    }

    private async Task ProcessTariffSelection()
    {
        if (SelectedTariff == null)
        {
            await ShowErrorAsync("Выберите тариф");
            return;
        }

        try
        {
            if (!await _visitService.IsClientActiveAsync(CurrentClient.ClientId))
            {
                await ShowErrorAsync("Клиент не имеет активного статуса");
                return;
            }

            if (await _visitService.IsClientAlreadyEnteredAsync(CurrentClient.ClientId))
            {
                await ShowErrorAsync("Ошибка. Вход уже осуществлен");
                return;
            }

            if (!await _workingHoursService.IsWorkingHoursAsync())
            {
                await ShowErrorAsync("Регистрация невозможна в нерабочее время");
                return;
            }

            await _visitService.EnterClientAsync(CurrentClient.ClientId, SelectedTariff.TariffId);

            CurrentState = EntryState.Success;
            ErrorMessage = string.Empty;
            UpdateStateProperties();

            CountdownSeconds = 15;
            _countdownTimer.Start();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Ошибка создания посещения: {ex.Message}");
        }
    }

    private async Task ShowErrorAsync(string message)
    {
        ErrorMessage = message;

        // Показываем TeachingTip с ошибкой через code-behind
        if (App.MainWindow?.Content is FrameworkElement rootElement)
        {
            // Находим EntryPage в дереве элементов
            var entryPage = FindEntryPage(rootElement);
            if (entryPage != null)
            {
                entryPage.ShowError(message);
            }
        }
    }

    private EntryPage? FindEntryPage(DependencyObject element)
    {
        if (element is EntryPage entryPage)
            return entryPage;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            var result = FindEntryPage(child);
            if (result != null)
                return result;
        }

        return null;
    }

    [RelayCommand]
    private void SkipWait()
    {
        _countdownTimer.Stop();
        CurrentState = EntryState.Welcome;
        ClearData();
        UpdateStateProperties();
    }

    private void CountdownTimer_Tick(object sender, object e)
    {
        CountdownSeconds--;
        if (CountdownSeconds <= 0)
        {
            _countdownTimer.Stop();
            SkipWait();
        }
    }

    private void UpdateStateProperties()
    {
        OnPropertyChanged(nameof(IsWelcomeState));
        OnPropertyChanged(nameof(IsCardInputState));
        OnPropertyChanged(nameof(IsRegistrationState));
        OnPropertyChanged(nameof(IsTariffSelectionState));
        OnPropertyChanged(nameof(IsSuccessState));
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoNext));
    }

    partial void OnCardNumberChanged(string value)
    {
        OnPropertyChanged(nameof(CanGoNext));
        UpdateStateProperties();
    }

    partial void OnSelectedTariffChanged(Tariff value)
    {
        System.Diagnostics.Debug.WriteLine($"SelectedTariff changed to: {value?.TariffName}");
        OnPropertyChanged(nameof(SelectedTariff));
        OnPropertyChanged(nameof(CanGoNext));
        UpdateStateProperties();
    }

    partial void OnFirstNameChanged(string value)
    {
        OnPropertyChanged(nameof(CanGoNext));
        UpdateStateProperties();
    }

    partial void OnLastNameChanged(string value)
    {
        OnPropertyChanged(nameof(CanGoNext));
        UpdateStateProperties();
    }

    partial void OnPhoneNumberChanged(string value)
    {
        OnPropertyChanged(nameof(CanGoNext));
        UpdateStateProperties();
    }


}