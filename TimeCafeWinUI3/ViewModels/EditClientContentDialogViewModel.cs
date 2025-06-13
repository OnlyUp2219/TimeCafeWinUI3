using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TimeCafeWinUI3.Core.Models;
using TimeCafeWinUI3.Core.Contracts.Services;
using System.Text;
using System.Threading.Tasks;

namespace TimeCafeWinUI3.ViewModels;

public partial class EditClientContentDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string firstName;

    [ObservableProperty]
    private string lastName;

    [ObservableProperty]
    private string middleName;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string phoneNumber;

    [ObservableProperty]
    private DateOnly? birthDate;

    [ObservableProperty]
    private int? genderId;

    [ObservableProperty]
    private ObservableCollection<Gender> genders = new();

    [ObservableProperty]
    private string errorMessage;

    private Client _client;
    private readonly IClientService _clientService;
    private string _originalPhoneNumber;

    public EditClientContentDialogViewModel(IClientService clientService)
    {
        _clientService = clientService;
        LoadGendersAsync();
    }

    private async void LoadGendersAsync()
    {
        var genders = await _clientService.GetGendersAsync();
        Genders.Clear();
        foreach (var gender in genders)
        {
            Genders.Add(gender);
        }
    }

    public void SetClient(Client client)
    {
        _client = client;
        FirstName = client.FirstName;
        LastName = client.LastName;
        MiddleName = client.MiddleName;
        Email = client.Email;
        PhoneNumber = client.PhoneNumber;
        _originalPhoneNumber = client.PhoneNumber;
        BirthDate = client.BirthDate;
        GenderId = client.GenderId;
    }

    public bool IsPhoneNumberChanged()
    {
        return PhoneNumber != _originalPhoneNumber;
    }

    public async Task<string> ValidateAsync()
    {
        var sb = new StringBuilder();

        if (string.IsNullOrWhiteSpace(FirstName))
            sb.AppendLine("Имя обязательно для заполнения");

        if (!string.IsNullOrWhiteSpace(Email))
        {
            var validMail = await _clientService.ValidateEmailAsync(Email);
            if (validMail)
                sb.AppendLine("Неверный формат email");
        }

        if (!string.IsNullOrWhiteSpace(PhoneNumber))
        {
            var validPhone = await _clientService.ValidatePhoneNumberAsync(PhoneNumber);
            if (!validPhone)
                sb.AppendLine("Неверный формат номера телефона или такой номер уже существует");
        }

        return sb.ToString();
    }

    public Client GetUpdatedClient()
    {
        if (_client == null)
            return null;

        _client.FirstName = FirstName;
        _client.LastName = LastName;
        _client.MiddleName = MiddleName;
        _client.Email = Email;
        _client.PhoneNumber = PhoneNumber;
        _client.BirthDate = BirthDate;
        _client.GenderId = GenderId;

        return _client;
    }

} 