namespace TimeCafeWinUI3.ViewModels;

public partial class CreateTariffViewModel : ObservableRecipient, INavigationAware
{

    [ObservableProperty] private string title;
    [ObservableProperty] private string type;
    [ObservableProperty] private string price;
    [ObservableProperty] private string descriptionTitle;
    [ObservableProperty] private string description;
    [ObservableProperty] private List<string> tariffTypes = new List<string> { "Базовый", "Премиум" };
    [ObservableProperty] private List<string> tariffThemes = new List<string> { "Светлая", "Темная" };

    public CreateTariffViewModel()
    {

    }

    public void OnNavigatedFrom()
    {

    }

    public void OnNavigatedTo(object parameter)
    {

    }
}