using Microsoft.UI.Xaml.Controls;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Views;

public sealed partial class EditClientContentDialog : Page
{
    public Client Client { get; private set; }

    public EditClientContentDialog()
    {
        this.InitializeComponent();
    }

    public void SetClient(Client client)
    {
        Client = client;

        // Заполняем поля текущими данными
        FirstNameTextBox.Text = client.FirstName;
        LastNameTextBox.Text = client.LastName;
        MiddleNameTextBox.Text = client.MiddleName;
        EmailTextBox.Text = client.Email;
        PhoneNumberTextBox.Text = client.PhoneNumber;
        if (client.BirthDate.HasValue)
        {
            BirthDatePicker.Date = new DateTimeOffset(client.BirthDate.Value.ToDateTime(TimeOnly.MinValue));
        }
        if (client.GenderId.HasValue)
        {
            GenderComboBox.SelectedValue = client.GenderId.Value;
        }
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Проверяем обязательные поля
        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
        {
            args.Cancel = true;
            return;
        }

        Client.FirstName = FirstNameTextBox.Text;
        Client.LastName = LastNameTextBox.Text;
        Client.MiddleName = MiddleNameTextBox.Text;
        Client.Email = EmailTextBox.Text;
        Client.PhoneNumber = PhoneNumberTextBox.Text;
        
        if (BirthDatePicker.Date != DateTimeOffset.MinValue)
        {
            Client.BirthDate = DateOnly.FromDateTime(BirthDatePicker.Date.DateTime);
        }
        else
        {
            Client.BirthDate = null;
        }
        
        // Обновляем пол
        if (GenderComboBox.SelectedValue != null)
        {
            Client.GenderId = (int)GenderComboBox.SelectedValue;
        }
        else
        {
            Client.GenderId = null;
        }
    }
} 