using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace TimeCafeWinUI3.ViewModels;

public partial class UserGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly IClientQueries _clientQueries;
    private static int _currentPage = 1;
    private const int PageSize = 16;

    [ObservableProperty]
    private ObservableCollection<Client> source = new();

    [ObservableProperty]
    private int totalItems;

    [ObservableProperty]
    private bool isLoading;

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }
    }

    public UserGridViewModel(INavigationService navigationService, IClientQueries clientQueries)
    {
        _navigationService = navigationService;
        _clientQueries = clientQueries;
    }

    [RelayCommand]
    private void AnimItemClick(Client? clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(UserGridDetailViewModel).FullName!, clickedItem);
        }
    }

    public async void OnNavigatedTo(object parameter)
    {
        // TODO : Аккуратно, правил GPT
        if (Source.Count == 0)
        {
            await LoadDataAsync();
        }
    }

    public void OnNavigatedFrom()
    {
        // TODO : Аккуратно, правил GPT
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Source.Clear();

            var (items, total) = await _clientQueries.GetClientsPageAsync(CurrentPage, PageSize);
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

    public async Task SetCurrentPage(int pageNumber)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (CurrentPage != pageNumber)
        {
            try
            {
                IsLoading = true;
                CurrentPage = pageNumber;
                Source.Clear();

                var (items, total) = await _clientQueries.GetClientsPageAsync(CurrentPage, PageSize);
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
