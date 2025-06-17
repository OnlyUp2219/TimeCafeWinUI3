using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Text;
using TimeCafeWinUI3.Contracts.Services;
using TimeCafeWinUI3.Core.Models;
using TimeCafeWinUI3.Views;

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

    public EntryViewModel(INavigationService navigationService, IClientService clientService, ITariffService tariffService)
    {
        _navigationService = navigationService;
        _clientService = clientService;
        _tariffService = tariffService;

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
            // Load tariffs
            var tariffs = await _tariffService.GetAllTariffsAsync();
            Tariffs.Clear();
            foreach (var tariff in tariffs.Take(10)) // Take first 10 tariffs
            {
                Tariffs.Add(tariff);
            }

            // Load genders
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

    // State properties
    public bool IsWelcomeState => CurrentState == EntryState.Welcome;
    public bool IsCardInputState => CurrentState == EntryState.CardInput;
    public bool IsRegistrationState => CurrentState == EntryState.Registration;
    public bool IsTariffSelectionState => CurrentState == EntryState.TariffSelection;
    public bool IsSuccessState => CurrentState == EntryState.Success;

    // Navigation properties
    public bool CanGoBack => CurrentState != EntryState.Welcome;
    public bool CanGoNext => CanProceedToNext();

    private bool CanProceedToNext()
    {
        return CurrentState switch
        {
            EntryState.Welcome => false, // No next button on welcome
            EntryState.CardInput => !string.IsNullOrWhiteSpace(CardNumber), // Show button when card number is entered
            EntryState.Registration => IsRegistrationValidSync(),
            EntryState.TariffSelection => SelectedTariff != null,
            EntryState.Success => true, // Always can skip wait
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
            ErrorMessage = "Введите номер карты СКУД";
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
                ErrorMessage = "Такого клиента нет или карты СКУД не правильная";
                return;
            }

            if (CurrentClient.StatusId != (int)ClientStatusType.Active)
            {
                ErrorMessage = "Клиент не в статусе активный";
                return;
            }

            CurrentState = EntryState.TariffSelection;
            ErrorMessage = string.Empty;
            UpdateStateProperties();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка поиска клиента: {ex.Message}";
        }
    }

    private async Task ProcessRegistration()
    {
        var validationResult = await ValidateRegistration();
        if (!string.IsNullOrEmpty(validationResult))
        {
            ErrorMessage = validationResult;
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
                // Show error for draft status
                ErrorMessage = "Клиент не в статусе активный";
                return; // Don't proceed to tariff selection
            }
            else
            {
                return; // User cancelled
            }

            ErrorMessage = string.Empty;
            UpdateStateProperties();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка регистрации: {ex.Message}";
        }
    }

    private async Task<string> ValidateRegistration()
    {
        var sb = new StringBuilder();

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            sb.AppendLine("Имя и фамилия обязательны для заполнения");

        if (string.IsNullOrWhiteSpace(PhoneNumber))
            sb.AppendLine("Номер телефона обязателен для заполнения");

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
            ErrorMessage = "Выберите тариф";
            return;
        }

        try
        {
            // TODO: Create visit record
            // For now, just show success
            CurrentState = EntryState.Success;
            ErrorMessage = string.Empty;
            UpdateStateProperties();

            // Start countdown
            CountdownSeconds = 15;
            _countdownTimer.Start();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка создания посещения: {ex.Message}";
        }
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