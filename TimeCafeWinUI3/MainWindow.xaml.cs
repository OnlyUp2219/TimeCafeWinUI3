using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.UI.Utilities.Extensions;
using TimeCafeWinUI3.UI.Utilities.Helpers;
using Windows.UI.ViewManagement;

namespace TimeCafeWinUI3.UI;

public sealed partial class MainWindow : WindowEx
{
    private Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;
    private UISettings settings;

    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();

        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event

        AppWindow.Closing += AppWindow_Closing;
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit!;
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("CurrentDomain_ProcessExit: Начало очистки ресурсов");

            if (settings != null)
            {
                settings.ColorValuesChanged -= Settings_ColorValuesChanged;
                settings = null!;
            }

            dispatcherQueue = null!;
            System.Diagnostics.Debug.WriteLine("CurrentDomain_ProcessExit: Ресурсы очищены");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CurrentDomain_ProcessExit: Ошибка при очистке ресурсов: {ex.Message}");
        }
    }

    private async void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        try
        {
            args.Cancel = true;

            System.Diagnostics.Debug.WriteLine("Начинаем закрытие окна...");

            var themeSelectorService = App.GetService<IThemeSelectorService>();
            var currentTheme = themeSelectorService.Theme;

            // Проверяем, есть ли активные посетители
            var visitQueries = App.GetService<IVisitQueries>();
            var activeVisits = await visitQueries.GetActiveVisitsAsync();
            var activeVisitorsCount = activeVisits.Count();

            string dialogContent;
            if (activeVisitorsCount > 0)
            {
                dialogContent = $"В заведении находится {activeVisitorsCount} активных посетителей.\n\n" +
                               "Убедитесь, что все посетители вышли из заведения перед закрытием приложения.\n\n" +
                               "Вы уверены, что хотите закрыть приложение?";
            }
            else
            {
                dialogContent = "Вы уверены, что хотите закрыть приложение?";
            }

            var dialog = new ContentDialog
            {
                Title = "Подтверждение",
                Content = dialogContent,
                PrimaryButtonText = "Да",
                SecondaryButtonText = "Нет",
                XamlRoot = Content.XamlRoot,
                RequestedTheme = currentTheme
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                System.Diagnostics.Debug.WriteLine("Пользователь подтвердил закрытие, скрываем окно");

                // TODO: Автоматический выход всех посетителей при закрытии
                // if (activeVisitorsCount > 0)
                // {
                //     await visitService.ExitAllVisitorsAsync("Закрытие приложения");
                // }

                // Принудительно закрываем соединение с БД
                try
                {
                    var db = App.GetService<TimeCafeWinUI3.UI.Core.Models.TimeCafeContext>();
                    db.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка при закрытии БД: {ex.Message}");
                }

                // AppWindow.Hide();
                Environment.Exit(0);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Пользователь отменил закрытие");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при закрытии окна: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

            try
            {
                // AppWindow.Hide();
                Environment.Exit(0);
            }
            catch (Exception exitEx)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при принудительном закрытии: {exitEx.Message}");
            }
        }
    }

    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }
}
