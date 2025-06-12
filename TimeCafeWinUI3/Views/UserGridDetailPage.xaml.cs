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

    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        var editClient = App.GetService<EditClientContentDialog>();
        editClient.SetClient(ViewModel.Item);

        var dialog = new ContentDialog
        {
            XamlRoot = App.MainWindow.Content.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
            Title = "Подтверждение телефона",
            PrimaryButtonText = "Подтвердить",
            SecondaryButtonText = "Пропустить",
            CloseButtonText = "Отмена",
            DefaultButton = ContentDialogButton.Primary,
            Content = editClient
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            // Здесь будет логика сохранения
        }
    }

    private async void RefuseButton_Click(object sender, RoutedEventArgs e)
    {
        var editClient = App.GetService<RefuseServiceContentDialog>();

        var dialog = new ContentDialog
        {
            XamlRoot = App.MainWindow.Content.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            RequestedTheme = App.GetService<IThemeSelectorService>().Theme,
            Title = "Подтверждение телефона",
            PrimaryButtonText = "Подтвердить",
            SecondaryButtonText = "Пропустить",
            CloseButtonText = "Отмена",
            DefaultButton = ContentDialogButton.Primary,
            Content = editClient
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            // Здесь будет логика обработки отказа
        }
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Общая логика для обоих диалогов
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
                //animation.Completed += Animation_Completed;
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
       ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackConnectedAnimation", detailedImage);

        //if (e.NavigationMode == NavigationMode.Back)
        //{
        //    var anim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("BackConnectedAnimation", detailedImage);
        //    if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        //    {
        //        anim.Configuration = new DirectConnectedAnimationConfiguration();
        //    }
        //}
    }
}
