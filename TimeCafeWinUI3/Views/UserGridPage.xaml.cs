using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Xaml.Interactivity;
using TimeCafeWinUI3.Contracts.Services;

namespace TimeCafeWinUI3.Views;

public sealed partial class UserGridPage : Page
{
    public UserGridViewModel ViewModel
    {
        get;
    }
    public UserGridPage()
    {
        ViewModel = App.GetService<UserGridViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var navigationService = App.GetService<INavigationService>();
    }

    private void AdaptiveGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.ItemContainer is GridViewItem gridViewItem)
        {

            Interaction.GetBehaviors(gridViewItem).Clear();
            Interaction.GetBehaviors(gridViewItem).Add(new SelectedPointerOverBehavior());
        }
    }

    private void OnPageChanged(object sender, int pageNumber)
    {
        if (ViewModel?.Source == null) return;

        var pageSize = PaginationControl.PageSize;
        var startIndex = (pageNumber - 1) * pageSize;
        var pageItems = ViewModel.Source.Skip(startIndex).Take(pageSize).ToList();

        // Обновляем ItemsSource AdaptiveGridView
        AdaptiveGrid.ItemsSource = pageItems;
    }
}
