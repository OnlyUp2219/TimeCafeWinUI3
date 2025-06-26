using CommunityToolkit.Mvvm.ComponentModel;
using TimeCafeWinUI3.Contracts.Services;

namespace TimeCafeWinUI3.ViewModels;

public partial class HelpViewModel : ObservableRecipient, INavigationAware
{
    public HelpViewModel() { }
    

    public void OnNavigatedTo(object parameter)
    {

    }

    public void OnNavigatedFrom()
    {
        // Логика при уходе со страницы справки (если потребуется)
    }
} 