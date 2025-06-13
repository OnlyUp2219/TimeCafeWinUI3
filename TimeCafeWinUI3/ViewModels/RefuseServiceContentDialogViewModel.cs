using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeCafeWinUI3.ViewModels;

public partial class RefuseServiceContentDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string reason;

    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(Reason);
    }
} 