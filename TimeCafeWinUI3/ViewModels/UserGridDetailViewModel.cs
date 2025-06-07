
namespace TimeCafeWinUI3.ViewModels;

public partial class UserGridDetailViewModel : ObservableRecipient, INavigationAware
{

    [ObservableProperty]
    private Client? item;

    public UserGridDetailViewModel()
    {
    }

    public async void OnNavigatedTo(object parameter)
    {

    }

    public void OnNavigatedFrom()
    {
    }
}
