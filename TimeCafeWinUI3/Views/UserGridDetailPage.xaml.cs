using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using TimeCafeWinUI3.Core.Models;
using TimeCafeWinUI3.ViewModels;

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
                animation.Completed += Animation_Completed;
                animation.TryStart(detailedImage, new UIElement[] { title, propertiesGroup1, propertiesGroup2 });
            }
        }
    }

    private void Animation_Completed(ConnectedAnimation sender, object args)
    {
        var anim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackConnectedAnimation", detailedImage);
        if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            anim.Configuration = new DirectConnectedAnimationConfiguration();
        }
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        if (e.NavigationMode == NavigationMode.Back)
        {
            var anim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackConnectedAnimation", detailedImage);
            if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                anim.Configuration = new DirectConnectedAnimationConfiguration();
            }
        }
    }
}
