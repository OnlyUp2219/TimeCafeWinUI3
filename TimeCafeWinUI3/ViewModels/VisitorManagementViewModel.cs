using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace TimeCafeWinUI3.ViewModels;

public partial class VisitorManagementViewModel : ObservableRecipient, INavigationAware
{
    private readonly IClientService _clientService;
    private readonly ITariffService _tariffService;
    private readonly IVisitService _visitService;
    private readonly DispatcherTimer _updateTimer;

    [ObservableProperty] private ObservableCollection<Visit> visitors = new();
    [ObservableProperty] private ObservableCollection<Visit> filteredVisitors = new();
    [ObservableProperty] private ObservableCollection<Tariff> availableTariffs = new();
    [ObservableProperty] private string searchText;
    [ObservableProperty] private Tariff selectedTariffFilter;
    [ObservableProperty] private bool isLoading;

    public VisitorManagementViewModel(IClientService clientService, ITariffService tariffService, IVisitService visitService)
    {
        _clientService = clientService;
        _tariffService = tariffService;
        _visitService = visitService;

        _updateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _updateTimer.Tick += UpdateTimer_Tick;
    }

    public async void OnNavigatedTo(object parameter)
    {
        await LoadDataAsync();
        _updateTimer.Start();
    }

    public void OnNavigatedFrom()
    {
        _updateTimer.Stop();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;

            // Load active visits (visits without exit time)
            await LoadActiveVisits();

            // Load available tariffs for filtering
            var tariffs = await _tariffService.GetAllTariffsAsync();
            AvailableTariffs.Clear();
            AvailableTariffs.Add(new Tariff { TariffId = 0, TariffName = "Все тарифы" }); // Add "All" option
            foreach (var tariff in tariffs)
            {
                AvailableTariffs.Add(tariff);
            }

            ApplyFilters();
        }
        catch (Exception ex)
        {
            // TODO: Show error message
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadActiveVisits()
    {
        try
        {
            var activeVisits = await _visitService.GetActiveVisitsAsync();
            Visitors.Clear();
            foreach (var visit in activeVisits)
            {
                Visitors.Add(visit);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки активных посещений: {ex.Message}");
        }
    }

    private void UpdateTimer_Tick(object sender, object e)
    {
        // Update duration and cost for all visitors
        foreach (var visit in Visitors)
        {
            UpdateVisitInfo(visit);
        }

        OnPropertyChanged(nameof(TotalVisitors));
        OnPropertyChanged(nameof(HasVisitors));
    }

    private void UpdateVisitInfo(Visit visit)
    {
        var duration = DateTime.Now - visit.EntryTime;
        visit.DurationText = FormatDuration(duration);
        visit.CurrentCost = CalculateCurrentCost(visit, duration);
    }

    private string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalHours >= 1)
        {
            return $"{(int)duration.TotalHours}ч {duration.Minutes}м";
        }
        return $"{duration.Minutes}м {duration.Seconds}с";
    }

    private decimal CalculateCurrentCost(Visit visit, TimeSpan duration)
    {
        if (visit.Tariff == null || visit.BillingType == null) return 0;

        switch (visit.BillingType.BillingTypeId)
        {
            case 1: // Почасовая тарификация
                // Почасовая тарификация с округлением вверх
                var hours = Math.Ceiling(duration.TotalHours);
                return visit.Tariff.Price * (decimal)hours;

            case 2: // Поминутная тарификация
                // Поминутная тарификация
                var minutes = duration.TotalMinutes;
                return visit.Tariff.Price * (decimal)minutes;

            default:
                return 0;
        }
    }

    private void ApplyFilters()
    {
        var filtered = Visitors.AsEnumerable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(v =>
                v.Client?.FirstName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                v.Client?.LastName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
        }

        // Apply tariff filter
        if (SelectedTariffFilter != null && SelectedTariffFilter.TariffId != 0)
        {
            filtered = filtered.Where(v => v.Tariff?.TariffId == SelectedTariffFilter.TariffId);
        }

        FilteredVisitors.Clear();
        foreach (var visit in filtered)
        {
            FilteredVisitors.Add(visit);
        }
    }

    public async Task ExitVisitorAsync(Visit visit)
    {
        if (visit == null) return;

        var dialog = new ContentDialog
        {
            Title = "Подтверждение выхода",
            Content = $"Подтвердите выход посетителя {visit.Client?.FirstName} {visit.Client?.LastName}",
            PrimaryButtonText = "Подтвердить",
            CloseButtonText = "Отмена",
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            try
            {
                await _visitService.ExitClientAsync(visit.VisitId);
                await LoadActiveVisits(); // Перезагружаем список
                ApplyFilters();
            }
            catch (Exception ex)
            {
                // Показываем ошибку пользователю
                var errorDialog = new ContentDialog
                {
                    Title = "Ошибка",
                    Content = $"Ошибка при выходе посетителя: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
                System.Diagnostics.Debug.WriteLine($"Ошибка при выходе посетителя: {ex.Message}");
            }
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnSelectedTariffFilterChanged(Tariff value)
    {
        ApplyFilters();
    }

    public int TotalVisitors => Visitors.Count;
    public bool HasVisitors => Visitors.Count > 0;
}