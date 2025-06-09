using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services;

public class ClientService : IClientService
{
    private readonly Dictionary<string, string> _phoneConfirmationCodes = new();
    private readonly Dictionary<int, bool> _confirmedPhones = new();

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        using var context = new TimeCafeContext();
        return await context.Clients
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Client> Items, int TotalCount)> GetClientsPageAsync(int pageNumber, int pageSize)
    {
        using var context = new TimeCafeContext();
        var query = context.Clients
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Client> GetClientByIdAsync(int clientId)
    {
        using var context = new TimeCafeContext();
        return await context.Clients
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);
    }

    public async Task<Client> CreateClientAsync(Client client)
    {
        using var context = new TimeCafeContext();
        client.CreatedAt = DateTime.Now;

        context.Clients.Add(client);
        await context.SaveChangesAsync();
        return client;
    }

    public async Task<Client> UpdateClientAsync(Client client)
    {
        using var context = new TimeCafeContext();
        var existingClient = await context.Clients.FindAsync(client.ClientId);
        if (existingClient == null)
            throw new KeyNotFoundException($"Клиент с ID {client.ClientId} не найден");

        context.Entry(existingClient).CurrentValues.SetValues(client);
        await context.SaveChangesAsync();
        return client;
    }

    public async Task<bool> DeleteClientAsync(int clientId)
    {
        using var context = new TimeCafeContext();
        var client = await context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        context.Clients.Remove(client);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidatePhoneNumberAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        using var context = new TimeCafeContext();
        if (await context.Clients.AnyAsync(c => c.PhoneNumber == phoneNumber))
            return false;

        var phoneRegex = new Regex(@"^\+375 \(\d{2}\) \d{3} \d{4}$");
        return phoneRegex.IsMatch(phoneNumber);
    }

    public async Task<bool> ValidateEmailAsync(string email)
    {
        var atCount = email.Count(c => c == '@');
        if (atCount != 1)
            return false;

        using var context = new TimeCafeContext();
        return !await context.Clients.AnyAsync(c => c.Email == email);
    }

    public async Task<bool> ValidateAccessCardNumberAsync(string accessCardNumber)
    {
        if (string.IsNullOrWhiteSpace(accessCardNumber))
            return true;

        if (accessCardNumber.Length != 20)
            return false;

        using var context = new TimeCafeContext();
        return !await context.Clients.AnyAsync(c => c.AccessCardNumber == accessCardNumber);
    }

    public async Task<IEnumerable<ClientStatus>> GetClientStatusesAsync()
    {
        using var context = new TimeCafeContext();
        return await context.ClientStatuses.ToListAsync();
    }

    public async Task<IEnumerable<Gender>> GetGendersAsync()
    {
        using var context = new TimeCafeContext();
        return await context.Genders.ToListAsync();
    }

    public async Task<bool> SendPhoneConfirmationCodeAsync(string phoneNumber)
    {
        if (!await ValidatePhoneNumberAsync(phoneNumber))
            return false;

        var code = new Random().Next(100000, 999999).ToString();
        _phoneConfirmationCodes[phoneNumber] = code;
        return true;
    }

    public async Task<bool> VerifyPhoneConfirmationCodeAsync(string phoneNumber, string code)
    {
        if (!_phoneConfirmationCodes.TryGetValue(phoneNumber, out var storedCode))
            return false;

        if (storedCode != code)
            return false;

        using var context = new TimeCafeContext();
        var client = await context.Clients.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
        if (client != null)
        {
            _confirmedPhones[client.ClientId] = true;
            await UpdateClientStatusAsync(client.ClientId, (int)ClientStatusType.Active);
        }

        _phoneConfirmationCodes.Remove(phoneNumber);
        return true;
    }

    public async Task<bool> IsPhoneConfirmedAsync(int clientId)
    {
        if (_confirmedPhones.TryGetValue(clientId, out var confirmed))
            return confirmed;

        using var context = new TimeCafeContext();
        var client = await context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        var isConfirmed = client.StatusId == (int)ClientStatusType.Active;
        _confirmedPhones[clientId] = isConfirmed;
        return isConfirmed;
    }

    public async Task<bool> UpdateClientStatusAsync(int clientId, int statusId)
    {
        using var context = new TimeCafeContext();
        var client = await context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        client.StatusId = statusId;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GenerateAccessCardNumberAsync()
    {
        using var context = new TimeCafeContext();
        var random = new Random();
        var cardNumber = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 20)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        while (await context.Clients.AnyAsync(c => c.AccessCardNumber == cardNumber))
        {
            cardNumber = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 20)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        return cardNumber;
    }
}