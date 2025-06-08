using CommunityToolkit.Mvvm.ComponentModel;
using TimeCafeWinUI3.Models;

namespace TimeCafeWinUI3.ViewModels;

public partial class CollectionViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string title = "Collection";

    public CollectionViewModel()
    {
    }
} 