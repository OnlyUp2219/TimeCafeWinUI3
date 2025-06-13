using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.ViewModels;

namespace TimeCafeWinUI3.Views;

public sealed partial class RefuseServiceContentDialog : Page
{
    public RefuseServiceContentDialogViewModel ViewModel { get; }

    public RefuseServiceContentDialog()
    {
        ViewModel = App.GetService<RefuseServiceContentDialogViewModel>();
        DataContext = ViewModel;
        this.InitializeComponent();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (!ViewModel.Validate())
        {
            args.Cancel = true;
            return;
        }
    }
} 