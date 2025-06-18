using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.Contracts.Services;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Services;

namespace TimeCafeWinUI3;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();
    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IClientAdditionalInfoService, ClientAdditionalInfoService>();
            services.AddTransient<ITariffService, TariffService>();
            services.AddTransient<IThemeService, ThemeService>();
            services.AddTransient<IThemeColorService, ThemeColorService>();
            services.AddTransient<IBillingTypeService, BillingTypeService>();
            services.AddTransient<IVisitService, VisitService>();
            services.AddTransient<IWorkingHoursService, WorkingHoursService>();
            services.AddDbContext<TimeCafeContext>(options => options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<CreateClientViewModel>();
            services.AddTransient<CreateClientPage>();
            services.AddTransient<EntryViewModel>();
            services.AddTransient<EntryPage>();
            services.AddTransient<VisitorManagementViewModel>();
            services.AddTransient<VisitorManagementPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<PhoneVerificationConfirm>();
            services.AddTransient<PhoneVerificationViewModel>();
            services.AddTransient<UserGridDetailPage>();
            services.AddTransient<UserGridDetailViewModel>();
            services.AddTransient<UserGridPage>();
            services.AddTransient<UserGridViewModel>();
            services.AddTransient<TariffManagePage>();
            services.AddTransient<TariffManageViewModel>();
            services.AddTransient<EditClientContentDialogViewModel>();
            services.AddTransient<RefuseServiceContentDialogViewModel>();
            services.AddTransient<CreateTariffViewModel>();
            services.AddTransient<CreateTariffPage>();

            // Register ContentDialogs
            services.AddTransient<EditClientContentDialog>();
            services.AddTransient<RefuseServiceContentDialog>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        UnhandledException += App_UnhandledException;

        Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "ru-RU";

        Window window = App.MainWindow;
        window.ExtendsContentIntoTitleBar = true;

        window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        using (var scope = Host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TimeCafeContext>();
            db.Database.EnsureCreated();
        }
    }

    private async void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Необработанное исключение: {e.Exception.GetType().FullName}");
        System.Diagnostics.Debug.WriteLine($"Сообщение: {e.Exception.Message}");
        System.Diagnostics.Debug.WriteLine($"StackTrace: {e.Exception.StackTrace}");
        System.Diagnostics.Debug.WriteLine($"Inner Exception: {e.Exception.InnerException?.Message}");
        System.Diagnostics.Debug.WriteLine($"Source: {e.Exception.Source}");

        try
        {
            string logPath = Path.Combine(AppContext.BaseDirectory, "error.log");
            string logMessage = $"[{DateTime.Now}]\n" +
                              $"Тип исключения: {e.Exception.GetType().FullName}\n" +
                              $"Сообщение: {e.Exception.Message}\n" +
                              $"StackTrace: {e.Exception.StackTrace}\n" +
                              $"Inner Exception: {e.Exception.InnerException?.Message}\n" +
                              $"Source: {e.Exception.Source}\n" +
                              "----------------------------------------\n";

            await File.AppendAllTextAsync(logPath, logMessage);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении лога: {ex.Message}");
        }

        if (MainWindow != null)
        {
            await MainWindow.DispatcherQueue.EnqueueAsync(async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = "Произошла ошибка",
                    Content = $"Тип: {e.Exception.GetType().Name}\n" +
                             $"Сообщение: {e.Exception.Message}\n\n" +
                             "Подробности сохранены в файл error.log",
                    PrimaryButtonText = "OK",
                    XamlRoot = MainWindow.Content.XamlRoot
                };

                await dialog.ShowAsync();
            });
        }

        // Предотвращаем завершение приложения
        e.Handled = true;
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        App.GetService<IAppNotificationService>().Show(string.Format(ResourceExtensions.GetLocalized("AppNotificationSamplePayload"), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);
    }

    // Обработчик закрытия приложения с проверкой активных посетителей
    protected async override void OnSuspending(SuspendingEventArgs args)
    {
        base.OnSuspending(args);

        try
        {
            using var scope = Host.Services.CreateScope();
            var visitService = scope.ServiceProvider.GetRequiredService<IVisitService>();
            
            // Проверяем, есть ли активные посетители
            var activeVisits = await visitService.GetActiveVisitsAsync();
            var activeVisitorsCount = activeVisits.Count();

            if (activeVisitorsCount > 0)
            {
                // Показываем предупреждение о том, что есть активные посетители
                await MainWindow.DispatcherQueue.EnqueueAsync(async () =>
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Внимание!",
                        Content = $"В заведении находится {activeVisitorsCount} активных посетителей.\n\n" +
                                 "Убедитесь, что все посетители вышли из заведения перед закрытием приложения.",
                        PrimaryButtonText = "Продолжить закрытие",
                        CloseButtonText = "Отмена",
                        XamlRoot = MainWindow.Content.XamlRoot
                    };

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        // TODO: Автоматический выход всех посетителей
                        // await visitService.ExitAllVisitorsAsync("Закрытие приложения");
                    }
                    else
                    {
                        // Отменяем закрытие приложения
                        args.SuspendingOperation.GetDeferral().Complete();
                    }
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при проверке активных посетителей: {ex.Message}");
        }
    }
}
public static class CrossManager
{
    public static List<CrossInfo> GeneratedCrosses { get; } = new List<CrossInfo>();
    public static bool IsInitialized { get; set; } = false;
}
