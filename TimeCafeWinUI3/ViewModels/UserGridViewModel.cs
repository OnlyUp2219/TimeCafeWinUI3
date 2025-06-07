using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TimeCafeWinUI3.Contracts.Services;

namespace TimeCafeWinUI3.ViewModels;

public partial class UserGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;
    public ObservableCollection<Client> Source { get; } = new();

    public UserGridViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

    }

    [RelayCommand]
    private void AnimItemClick(Client? clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(UserGridDetailViewModel).FullName!, clickedItem.PhoneNumber);
        }
    }

    public async void OnNavigatedTo(object parameter)
    {
    }

    public void OnNavigatedFrom()
    {
    }
}
