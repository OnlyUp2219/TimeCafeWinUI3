using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using TimeCafeWinUI3.Contracts.Services;
using TimeCafeWinUI3.Core.Contracts.Services;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimeCafeWinUI3.Views;

public sealed partial class TariffManagePage : Page
{
    private readonly IThemeColorService _themeColorService;
    public TariffManageViewModel ViewModel { get; }
    public TariffViewModel FakeViewModel { get; }
    public TariffManagePage()
    {
        ViewModel = App.GetService<TariffManageViewModel>();
        _themeColorService = App.GetService<IThemeColorService>();
        InitializeComponent();
        FakeViewModel = new TariffViewModel();
        DataContext = FakeViewModel;

        OnPageChanged(null!, 1);

        FakeViewModel.Tariffs.CollectionChanged += (s, e) =>
        {
            if (FakeViewModel.Tariffs.Count > 0)
            {
                OnPageChanged(null!, 1);
            }
        };

        AdaptiveGrid.ContainerContentChanging += (s, e) =>
        {
            if (e.Phase == 0)
            {
                if (e.ItemContainer is GridViewItem container)
                {
                    if (container.ContentTemplateRoot is Border border)
                    {
                        if (GradientSelector.SelectedItem is ComboBoxItem selectedItem)
                        {
                            string technicalName = selectedItem.Tag.ToString()!.Replace("GradientBrush", "");
                            border.Style = _themeColorService.GetThemeBorderStyle(technicalName);
                        }
                    }
                }
            }
        };

        this.Loaded += TariffManagePage_Loaded;

        var sv = FindScrollViewer(AdaptiveGrid);
        if (sv != null)
            sv.ViewChanged += (s, ev) => DispatcherQueue.TryEnqueue(ApplyToVisible);
    }
    private void TariffManagePage_Loaded(object sender, RoutedEventArgs e)
    {
        ApplyGradientStyle();
    }

    private void GradientSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyGradientStyle();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var navigationService = App.GetService<INavigationService>();

        // Принудительно обновляем страницу при возвращении
        TopPaginationControl.ForceUpdatePage();
    }

    private void ApplyGradientStyle()
    {
        if (AdaptiveGrid == null)
            return;

        if (GradientSelector.SelectedItem is ComboBoxItem selectedItem)
        {
            string technicalName = selectedItem.Tag.ToString()!.Replace("GradientBrush", "");
            var style = _themeColorService.GetThemeBorderStyle(technicalName);
            
            foreach (var item in AdaptiveGrid.Items)
            {
                if (AdaptiveGrid.ContainerFromItem(item) is GridViewItem container)
                {
                    if (container.ContentTemplateRoot is Border border)
                    {
                        border.Style = style;
                    }
                }
            }
        }
    }
    private void TariffBorder_Loaded(object sender, RoutedEventArgs e)
    {
        var border = (Border)sender;
        var canvas = (Canvas)border.FindName("CrossCanvas");
        if (canvas == null) return;

        CrossBlockPool.ReleaseAll(canvas);

        var tariff = (Tariff)border.DataContext;
        int idx = Math.Abs(tariff.TariffName.GetHashCode()) % CrossTemplates.Templates.Count;
        var template = CrossTemplates.Templates[idx];

        double w = border.ActualWidth;
        double h = border.ActualHeight;

        double scaleX = w / 400.0;
        double scaleY = h / 375.0;

        foreach (var item in template)
        {
            var cross = CrossBlockPool.Get();
            cross.FontSize = item.Size;

            cross.RenderTransform = new RotateTransform
            {
                Angle = item.Angle,
                CenterX = item.Size / 2,
                CenterY = item.Size / 2
            };


            Canvas.SetLeft(cross, item.Xpx * scaleX);
            Canvas.SetTop(cross, item.Ypx * scaleY);

            canvas.Children.Add(cross);
        }
    }

    private ScrollViewer FindScrollViewer(DependencyObject parent)
    {
        if (parent is ScrollViewer sv) return sv;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            var result = FindScrollViewer(child);
            if (result != null) return result;
        }
        return null;
    }

    private void ApplyToVisible()
    {
        var sv = FindScrollViewer(AdaptiveGrid);
        if (sv == null) return;

        double top = sv.VerticalOffset;
        double bottom = top + sv.ViewportHeight;

        foreach (var item in AdaptiveGrid.Items)
        {
            if (!(AdaptiveGrid.ContainerFromItem(item) is GridViewItem container)) continue;
            var transform = container.TransformToVisual(sv);
            var pos = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
            double itemTop = pos.Y;
            double itemBottom = pos.Y + container.ActualHeight;

            var border = container.ContentTemplateRoot as Border;
            if (border == null) continue;
            var canvas = border.FindName("CrossCanvas") as Canvas;
            if (canvas == null) continue;

            if (itemBottom >= 0 && itemTop <= sv.ViewportHeight)
            {
                // элемент видим — убедимся, что крестики нарисованы
                if (canvas.Children.Count == 0)
                    TariffBorder_Loaded(border, null);
            }
            else
            {
                // вне экрана — очистим и вернём в пул
                CrossBlockPool.ReleaseAll(canvas);
            }
        }
    }

    private void OnPageChanged(object sender, int pageNumber)
    {
        if (FakeViewModel?.Tariffs == null) return;

        // Получаем сохраненную позицию из PaginationControl
        var paginationControl = sender as PaginationControl ?? TopPaginationControl;
        var currentPage = paginationControl.CurrentPage;

        var pageSize = TopPaginationControl.PageSize;
        var startIndex = (currentPage - 1) * pageSize;
        var pageItems = FakeViewModel.Tariffs.Skip(startIndex).Take(pageSize).ToList();

        // Обновляем ItemsSource AdaptiveGridView
        AdaptiveGrid.ItemsSource = pageItems;

        // Синхронизируем состояние обоих контролов пагинации
        if (sender != TopPaginationControl)
        {
            TopPaginationControl.CurrentPage = currentPage;
        }
        if (sender != BottomPaginationControl)
        {
            BottomPaginationControl.CurrentPage = currentPage;
        }
    }
}



public class TariffViewModel
{
    public ObservableCollection<Tariff> Tariffs { get; set; }
    public int TotalItems { get; private set; }

    public TariffViewModel()
    {
        // Инициализируем пустую коллекцию
        Tariffs = new ObservableCollection<Tariff>();

        // Генерируем тарифы синхронно
        GenerateTariffs(50); // Уменьшим количество для тестирования
        TotalItems = Tariffs.Count;
    }

    private void GenerateTariffs(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            var tariff = new Tariff
            {
                TariffName = $"Тариф {i}",
                Price = i * 100,
                BillingTypeId = i % 2 == 0 ? 1 : 2,
                DescriptionTitle = i % 2 == 0 ? "Описание" : "Что включено",
                Description = $"Описание тарифа номер {i}. Уникальные особенности."
            };
            Tariffs.Add(tariff);
        }
    }
}

