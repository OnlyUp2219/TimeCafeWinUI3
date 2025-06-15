using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.ViewModels;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace TimeCafeWinUI3.Views;

public sealed partial class CreateTariffPage : Page
{
    public CreateTariffViewModel ViewModel { get; }

    public CreateTariffPage()
    {
        ViewModel = App.GetService<CreateTariffViewModel>();
        DataContext = ViewModel;
        this.InitializeComponent();
    }

    private async void SelectIcon_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            try
            {
                using var stream = await file.OpenStreamForReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ViewModel.Icon = ms.ToArray();
            }
            catch (Exception ex)
            {
                ViewModel.ErrorMessage = $"Ошибка при загрузке иконки: {ex.Message}";
            }
        }
    }
} 