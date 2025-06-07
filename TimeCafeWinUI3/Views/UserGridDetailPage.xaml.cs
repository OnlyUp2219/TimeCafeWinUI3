using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using TimeCafeWinUI3.Contracts.Services;


namespace TimeCafeWinUI3.Views;

public sealed partial class UserGridDetailPage : Page
{

    public UserGridDetailViewModel ViewModel
    {
        get;
    }
    public UserGridDetailPage()
    {
        ViewModel = App.GetService<UserGridDetailViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        this.RegisterElementForConnectedAnimation("listItem", itemHero1);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        if (e.NavigationMode == NavigationMode.Back)
        {
            var navigationService = App.GetService<INavigationService>();

            if (ViewModel.Item != null)
            {
                navigationService.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            }
        }
    }
}
