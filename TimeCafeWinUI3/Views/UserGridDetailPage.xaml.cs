using Bogus.DataSets;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
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
        DataContext = ViewModel;
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Client item)
        {
            ViewModel.Item = item;
            this.ViewModel.Item = item;

            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardConnectedAnimation");
            if (animation != null)
            {
                animation.TryStart(detailedImage, new UIElement[] { title, propertiesGroup1, propertiesGroup2 });
            }
        }
    }


    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackConnectedAnimation", detailedImage);


    }
}
