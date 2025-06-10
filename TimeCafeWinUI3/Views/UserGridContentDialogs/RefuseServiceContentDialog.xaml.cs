using Microsoft.UI.Xaml.Controls;

namespace TimeCafeWinUI3.Views;

public sealed partial class RefuseServiceContentDialog : Page
{
    public string Theme { get; private set; }
    public string Reason { get; private set; }

    public RefuseServiceContentDialog()
    {
        this.InitializeComponent();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Проверяем обязательные поля
        if (string.IsNullOrWhiteSpace(SubjectTextBox.Text) || string.IsNullOrWhiteSpace(ReasonTextBox.Text))
        {
            args.Cancel = true;
            return;
        }

        Theme = SubjectTextBox.Text;
        Reason = ReasonTextBox.Text;
    }
} 