using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Text;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;
using TimeCafeWinUI3.Contracts.Services;
using TimeCafeWinUI3.Contracts.ViewModels;

namespace TimeCafeWinUI3.ViewModels;

public partial class ClientFinanceViewModel : ObservableRecipient, INavigationAware
{
    private readonly IFinancialService _financialService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private Client currentClient;
    [ObservableProperty] private decimal currentBalance;
    [ObservableProperty] private decimal debtAmount;
    [ObservableProperty] private decimal depositAmount;
    [ObservableProperty] private string errorMessage;
    [ObservableProperty] private string successMessage;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private ObservableCollection<FinancialTransaction> transactions = new();

    public ClientFinanceViewModel(IFinancialService financialService, INavigationService navigationService)
    {
        _financialService = financialService;
        _navigationService = navigationService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is Client client)
        {
            CurrentClient = client;
            await LoadFinanceDataAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        ClearData();
    }

    private async Task LoadFinanceDataAsync()
    {
        if (CurrentClient == null) return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            // Загружаем баланс и задолженность
            CurrentBalance = await _financialService.GetClientBalanceAsync(CurrentClient.ClientId);
            DebtAmount = await _financialService.GetClientDebtAsync(CurrentClient.ClientId);

            // Загружаем историю транзакций
            var transactions = await _financialService.GetClientTransactionsAsync(CurrentClient.ClientId, 50);
            Transactions.Clear();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке финансовых данных: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DepositAsync()
    {
        if (CurrentClient == null) return;

        var validationResult = ValidateDeposit();
        if (!string.IsNullOrEmpty(validationResult))
        {
            ErrorMessage = validationResult;
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await _financialService.DepositAsync(CurrentClient.ClientId, DepositAmount, "Пополнение депозита");
            
            SuccessMessage = $"Депозит успешно пополнен на сумму {DepositAmount:C}";
            DepositAmount = 0;

            // Обновляем данные
            await LoadFinanceDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при пополнении депозита: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DepositFullAsync()
    {
        if (CurrentClient == null) return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var fullAmount = await _financialService.GetFullReplenishmentAmountAsync(CurrentClient.ClientId);
            if (fullAmount <= 0)
            {
                ErrorMessage = "У клиента нет задолженности для погашения";
                return;
            }

            DepositAmount = fullAmount;
            await DepositAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при расчете суммы погашения: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ClearMessages()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
    }

    private string ValidateDeposit()
    {
        var sb = new StringBuilder();

        if (DepositAmount <= 0)
            sb.AppendLine("Сумма пополнения должна быть больше 0");

        if (DepositAmount > 100000) // Максимальная сумма пополнения
            sb.AppendLine("Сумма пополнения не может превышать 100,000 ₽");

        return sb.ToString();
    }

    private void ClearData()
    {
        CurrentClient = null;
        CurrentBalance = 0;
        DebtAmount = 0;
        DepositAmount = 0;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        Transactions.Clear();
    }

    partial void OnDepositAmountChanged(decimal value)
    {
        // Форматируем сумму для отображения
        DepositAmount = Math.Round(value, 2);
    }
} 