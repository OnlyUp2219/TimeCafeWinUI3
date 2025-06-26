using Microsoft.UI.Xaml.Controls;
using Windows.System;
using TimeCafeWinUI3.ViewModels;
using TimeCafeWinUI3.Contracts.Services;

namespace TimeCafeWinUI3.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }

    private void HelpButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService.NavigateTo(typeof(HelpViewModel).FullName!);
    }

}
