using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;
using TimeCafeWinUI3.Contracts.Services;
using TimeCafeWinUI3.Contracts.ViewModels;
using TimeCafeWinUI3.Models;
using TimeCafeWinUI3.Views;

namespace TimeCafeWinUI3.ViewModels;

public partial class FinanceManagementViewModel : ObservableRecipient, INavigationAware
{
    private readonly IFinancialService _financialService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private ObservableCollection<ClientBalanceInfo> allClients = new();
    [ObservableProperty] private ObservableCollection<ClientBalanceInfo> filteredClients = new();
    [ObservableProperty] private string searchText;
    [ObservableProperty] private bool showOnlyDebtors;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string errorMessage;
    [ObservableProperty] private decimal totalDebt;
    [ObservableProperty] private int debtorsCount;

    public FinanceManagementViewModel(IFinancialService financialService, INavigationService navigationService)
    {
        _financialService = financialService;
        _navigationService = navigationService;
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
            ErrorMessage = string.Empty;

            var clientsData = await _financialService.GetAllClientsBalancesAsync();
            
            AllClients.Clear();
            foreach (dynamic clientData in clientsData)
            {
                var clientInfo = new ClientBalanceInfo
                {
                    ClientId = clientData.ClientId,
                    FullName = clientData.FullName,
                    PhoneNumber = clientData.PhoneNumber,
                    Balance = clientData.Balance,
                    Debt = clientData.Debt,
                    LastTransactionDate = clientData.LastTransactionDate,
                    IsActive = clientData.IsActive
                };
                AllClients.Add(clientInfo);
            }

            TotalDebt = AllClients.Sum(c => c.Debt);
            DebtorsCount = AllClients.Count(c => c.Debt > 0);

            ApplyFilters();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void RefreshData()
    {
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private void NavigateToClientDetail(ClientBalanceInfo client)
    {
        if (client != null)
        {
            _navigationService.NavigateTo(typeof(UserGridDetailViewModel).FullName!, client.ClientId);
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnShowOnlyDebtorsChanged(bool value)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = AllClients.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(c =>
                c.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                c.PhoneNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        if (ShowOnlyDebtors)
        {
            filtered = filtered.Where(c => c.Debt > 0);
        }

        FilteredClients.Clear();
        foreach (var client in filtered)
        {
            FilteredClients.Add(client);
        }
    }

    private void ClearData()
    {
        AllClients.Clear();
        FilteredClients.Clear();
        SearchText = string.Empty;
        ShowOnlyDebtors = false;
        TotalDebt = 0;
        DebtorsCount = 0;
        ErrorMessage = string.Empty;
    }
} 