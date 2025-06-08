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

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.RequestedTheme = App.GetService<IThemeSelectorService>().Theme;
        dialog.Title = "Вопрос";
        dialog.PrimaryButtonText = "Да";
        dialog.SecondaryButtonText = "Нет";
        dialog.CloseButtonText = "Отмена";
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.Content = new PhoneVerificationConfirm();

        var result = await dialog.ShowAsync();
    }

    private void MyAdaptiveGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.ItemContainer is GridViewItem gridViewItem)
        {
            System.Diagnostics.Debug.WriteLine("ContainerContentChanging called for GridViewItem");
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


