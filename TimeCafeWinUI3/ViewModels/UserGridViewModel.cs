using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TimeCafeWinUI3.Contracts.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using TimeCafeWinUI3.Core.Contracts.Services;

namespace TimeCafeWinUI3.ViewModels;

public partial class UserGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly IClientService _clientService;
    private int _currentPage = 1;
    private const int PageSize = 16;

    [ObservableProperty] 
    private ObservableCollection<Client> source = new();

    [ObservableProperty]
    private int totalItems;

    [ObservableProperty]
    private bool isLoading;

    public int CurrentPage => _currentPage;

    public UserGridViewModel(INavigationService navigationService, IClientService clientService)
    {
        _navigationService = navigationService;
        _clientService = clientService;
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

    public async Task SetCurrentPage(int pageNumber)
    {
        if (_currentPage != pageNumber)
        {
            try
            {
                IsLoading = true;
                _currentPage = pageNumber;
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
