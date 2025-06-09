using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System.Collections.ObjectModel;
using TimeCafeWinUI3.Contracts.Services;
using Windows.Media.Core;

namespace TimeCafeWinUI3.Views;

public sealed partial class CreateClientPage : Page
{
    public CreateClientViewModel ViewModel
    {
        get;
    }
    public ObservableCollection<TooltipItem> TooltipItems { get; } = new ObservableCollection<TooltipItem>();

    public CreateClientPage()
    {
        ViewModel = App.GetService<CreateClientViewModel>();
        DataContext = ViewModel;
        InitializeComponent();

        ViewModel.ListView = ListView;
        ViewModel.AdaptiveGrid = AdaptiveGrid;

        TooltipItems.Add(new TooltipItem
        {
            Title = "Заголовок подсказки 1",
            Description = "Это текстовое описание подсказки. Здесь можно описать, как работает функционал, или дать полезные советы.",
            MediaSource = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/HelpContent/sample1.mp4"))
        });

        TooltipItems.Add(new TooltipItem
        {
            Title = "Заголовок подсказки 2",
            Description = "Это второе описание подсказки с другим содержимым.",
            MediaSource = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/HelpContent/sample2.mp4"))
        });

        TooltipItems.Add(new TooltipItem
        {
            Title = "Заголовок подсказки 3",
            Description = "Это второе описание подсказки с другим содержимым.",
            MediaSource = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/HelpContent/sample2.mp4"))
        });

        Console.WriteLine($"MediaSource: {TooltipItems[0].MediaSource}");
    }

    private void MyAdaptiveGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.ItemContainer is GridViewItem gridViewItem)
        {
            Interaction.GetBehaviors(gridViewItem).Clear();
            Interaction.GetBehaviors(gridViewItem).Add(new SelectedPointerOverBehavior());
        }
    }

    private async void OnPageChanged(object sender, int pageNumber)
    {
        if (ViewModel?.Source == null) return;
        await ViewModel.SetCurrentPage(pageNumber);
    }
}


