using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace TimeCafeWinUI3.ViewModels;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class CreateTariffViewModel : ObservableRecipient, INavigationAware
{
    private readonly ITariffService _tariffService;
    private readonly IThemeService _themeService;
    private readonly IBillingTypeService _billingTypeService;
    private readonly IThemeColorService _themeColorService;
    private readonly FakeDataGenerator _fakeDataGenerator;

    [ObservableProperty] private string tariffName;
    [ObservableProperty] private byte[] icon;
    [ObservableProperty] private int? themeId;
    [ObservableProperty] private decimal price;
    [ObservableProperty] private int billingTypeId;
    [ObservableProperty] private string errorMessage;
    [ObservableProperty] private LinearGradientBrush selectedThemeBrush;
    [ObservableProperty] private string descriptionTitle;
    [ObservableProperty] private string description;
    [ObservableProperty] private string selectedBillingTypeName;
    [ObservableProperty] private Style selectedThemeStyle;

    [ObservableProperty] private ObservableCollection<Theme> themes = new();
    [ObservableProperty] private ObservableCollection<BillingType> billingTypes = new();

    public CreateTariffViewModel(
        ITariffService tariffService,
        IThemeService themeService,
        IBillingTypeService billingTypeService,
        IThemeColorService themeColorService)
    {
        _tariffService = tariffService;
        _themeService = themeService;
        _billingTypeService = billingTypeService;
        _themeColorService = themeColorService;
        _fakeDataGenerator = new FakeDataGenerator();
    }

    public async void OnNavigatedTo(object parameter)
    {
        await LoadDataAsync();
    }

    public void OnNavigatedFrom()
    {
        ClearData();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            Themes.Clear();
            BillingTypes.Clear();

            var themes = await _themeService.GetThemesAsync();
            foreach (var theme in themes)
            {
                Themes.Add(theme);
            }

            var billingTypes = await _billingTypeService.GetBillingTypesAsync();
            foreach (var billingType in billingTypes)
            {
                BillingTypes.Add(billingType);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
        }
    }

    private void ClearData()
    {
        TariffName = string.Empty;
        Icon = null;
        ThemeId = null;
        Price = 0;
        BillingTypeId = 0;
        ErrorMessage = string.Empty;
        DescriptionTitle = string.Empty;
        Description = string.Empty;
        SelectedBillingTypeName = string.Empty;
        SelectedThemeStyle = null;
    }

    public async Task<string> ValidateAsync()
    {
        var sb = new StringBuilder();

        if (string.IsNullOrWhiteSpace(TariffName))
            sb.AppendLine("Название тарифа обязательно для заполнения");

        if (Price <= 0)
            sb.AppendLine("Стоимость тарифа должна быть больше 0");

        if (BillingTypeId <= 0)
            sb.AppendLine("Тип тарифа обязателен для выбора");

        return sb.ToString();
    }

    [RelayCommand]
    private async Task CreateTariffAsync()
    {
        var validationResult = await ValidateAsync();
        if (!string.IsNullOrEmpty(validationResult))
        {
            ErrorMessage = validationResult;
            return;
        }

        try
        {
            var tariff = new Tariff
            {
                TariffName = TariffName,
                Icon = Icon,
                ThemeId = ThemeId,
                Price = Price,
                BillingTypeId = BillingTypeId,
                DescriptionTitle = DescriptionTitle,
                Description = Description,
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now
            };

            await _tariffService.CreateTariffAsync(tariff);
            ClearData();
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при создании тарифа: {ex.Message}";
        }
    }

    [RelayCommand]
    private void GenerateFakeData()
    {
        var fakeTariff = _fakeDataGenerator.GenerateTariff();

        TariffName = fakeTariff.TariffName;
        DescriptionTitle = fakeTariff.DescriptionTitle;
        Description = fakeTariff.Description;
        Price = fakeTariff.Price;

        // Выбираем случайный тип тарифа, если есть
        if (BillingTypes.Any())
        {
            var randomBillingType = BillingTypes[new Random().Next(BillingTypes.Count)];
            BillingTypeId = randomBillingType.BillingTypeId;
        }

        // Выбираем случайную тему, если есть
        if (Themes.Any())
        {
            var randomTheme = Themes[new Random().Next(Themes.Count)];
            ThemeId = randomTheme.ThemeId;
        }
    }

    partial void OnThemeIdChanged(int? value)
    {
        if (value.HasValue)
        {
            var theme = themes.FirstOrDefault(t => t.ThemeId == value);
            if (theme != null)
            {
                SelectedThemeBrush = _themeColorService.GetThemeGradientBrush(theme.TechnicalName);
                SelectedThemeStyle = _themeColorService.GetThemeBorderStyle(theme.TechnicalName);
            }
        }
        else
        {
            SelectedThemeBrush = null;
            SelectedThemeStyle = null;
        }
    }

    partial void OnBillingTypeIdChanged(int value)
    {
        var billingType = BillingTypes.FirstOrDefault(b => b.BillingTypeId == value);
        SelectedBillingTypeName = billingType?.BillingTypeName ?? string.Empty;
    }

    partial void OnPriceChanged(decimal value)
    {
        // Форматируем цену для отображения
        Price = Math.Round(value, 2);
        Debug.WriteLine(value);
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}