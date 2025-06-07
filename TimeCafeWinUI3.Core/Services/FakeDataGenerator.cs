using Bogus;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services;

public class FakeDataGenerator
{
    private readonly Faker<Client> _clientFaker;

    public FakeDataGenerator()
    {
        _clientFaker = new Faker<Client>("ru")
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.MiddleName, f => f.Name.FirstName())
            .RuleFor(c => c.GenderId, f => f.Random.Number(1, 2)) // 1 - мужской, 2 - женский
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.BirthDate, f => DateOnly.FromDateTime(f.Date.Past(80, DateTime.Now.AddYears(-18))))
            .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber("+375 (##) ### ####"))
            .RuleFor(c => c.AccessCardNumber, f => null )
            .RuleFor(c => c.CreatedAt, f => DateTime.Now);
    }

    public Client GenerateClient()
    {
        return _clientFaker.Generate();
    }
}