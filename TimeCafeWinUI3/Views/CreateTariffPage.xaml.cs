using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace TimeCafeWinUI3.Views;

public sealed partial class CreateTariffPage : Page
{
    public CreateTariffViewModel ViewModel { get; }

    public CreateTariffPage()
    {
        ViewModel = App.GetService<CreateTariffViewModel>();
        DataContext = ViewModel;
        this.InitializeComponent();

        this.SizeChanged += OnSizeChanged;
        UpdateVisualState();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        string state = ActualWidth >= 720 ? "WideLayout" : "NarrowLayout";
        VisualStateManager.GoToState(this, state, false);
        System.Diagnostics.Debug.WriteLine($"VisualState: {state}, Width: {ActualWidth}");
    }
} 