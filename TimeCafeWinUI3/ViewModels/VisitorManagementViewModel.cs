using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace TimeCafeWinUI3.ViewModels;

public partial class VisitorManagementViewModel : ObservableRecipient, INavigationAware
{
    private readonly IClientService _clientService;
    private readonly ITariffService _tariffService;
    private readonly DispatcherTimer _updateTimer;

    [ObservableProperty] private ObservableCollection<Visit> visitors = new();
    [ObservableProperty] private ObservableCollection<Visit> filteredVisitors = new();
    [ObservableProperty] private ObservableCollection<Tariff> availableTariffs = new();
    [ObservableProperty] private string searchText;
    [ObservableProperty] private Tariff selectedTariffFilter;
    [ObservableProperty] private bool isLoading;

    public VisitorManagementViewModel(IClientService clientService, ITariffService tariffService)
    {
        _clientService = clientService;
        _tariffService = tariffService;

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
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadActiveVisits()
    {
        // TODO: Implement VisitService to get active visits
        // For now, we'll simulate with some fake data
        Visitors.Clear();

        // This is temporary - replace with actual service call
        // var activeVisits = await _visitService.GetActiveVisitsAsync();
        // foreach (var visit in activeVisits)
        // {
        //     Visitors.Add(visit);
        // }

        // Temporary test data
        var testVisit = new Visit
        {
            VisitId = 1,
            EntryTime = DateTime.Now.AddHours(-1),
            Client = new Client
            {
                FirstName = "Иван",
                LastName = "Иванов",
                PhoneNumber = "+375 (29) 123 4567"
            },
            Tariff = new Tariff
            {
                TariffName = "Стандартный",
                Price = 100
            }
        };
        Visitors.Add(testVisit);

        var testVisit2 = new Visit
        {
            VisitId = 2,
            EntryTime = DateTime.Now.AddMinutes(-30),
            Client = new Client
            {
                FirstName = "Петр",
                LastName = "Петров",
                PhoneNumber = "+375 (33) 987 6543"
            },
            Tariff = new Tariff
            {
                TariffName = "Премиум",
                Price = 150
            }
        };
        Visitors.Add(testVisit2);
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
        if (visit.Tariff == null) return 0;

        // TODO: Implement proper billing calculation based on BillingType
        // For now, simple hourly calculation
        var hours = Math.Ceiling(duration.TotalHours);
        return visit.Tariff.Price * (decimal)hours;
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

    [RelayCommand]
    private async Task ExitVisitor(Visit visit)
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
                // TODO: Implement exit logic
                // await _visitService.ExitVisitorAsync(visit.VisitId);

                // For now, just remove from list
                Visitors.Remove(visit);
                ApplyFilters();
            }
            catch (Exception ex)
            {
                // TODO: Show error message
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