using CommunityToolkit.Mvvm.ComponentModel;
using TimeCafeWinUI3.Models;

namespace TimeCafeWinUI3.ViewModels;

public partial class DetailedInfoViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string title = "Detailed Info";

    public DetailedInfoViewModel()
    {
    }
} 