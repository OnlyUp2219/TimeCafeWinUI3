using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System.Reflection;
using System.Windows.Input;
using TimeCafeWinUI3.Contracts.Services;
using Windows.ApplicationModel;

namespace TimeCafeWinUI3.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        if (App.MainWindow?.Content is FrameworkElement rootElement)
        {
            rootElement.ActualThemeChanged += RootElement_ActualThemeChanged;
        }

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
    }

    private void RootElement_ActualThemeChanged(FrameworkElement sender, object args)
    {
        if (_themeSelectorService.Theme == ElementTheme.Default)
        {
            ElementTheme = sender.ActualTheme;
        }
        else
        {
            ElementTheme = _themeSelectorService.Theme;
        }
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
