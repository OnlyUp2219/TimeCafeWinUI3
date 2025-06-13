using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services;

public class ClientService : IClientService
{
    private readonly Dictionary<string, string> _phoneConfirmationCodes = new();
    private readonly Dictionary<int, bool> _confirmedPhones = new();
    private readonly TimeCafeContext _context;

    public ClientService(TimeCafeContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        return await _context.Clients
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Client> Items, int TotalCount)> GetClientsPageAsync(int pageNumber, int pageSize)
    {
        var items = await _context.Clients
            .AsNoTracking()
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        var totalCount = items.Count;

        return (items, totalCount);
    }

    public async Task<Client> GetClientByIdAsync(int clientId)
    {
        return await _context.Clients
            .Include(c => c.Status)
            .Include(c => c.Gender)
            .Include(c => c.ClientAdditionalInfos)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);
    }

    public async Task<Client> CreateClientAsync(Client client)
    {
        client.CreatedAt = DateTime.Now;
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<Client> UpdateClientAsync(Client client)
    {
        var existingClient = await _context.Clients.FindAsync(client.ClientId);
        if (existingClient == null)
            throw new KeyNotFoundException($"Клиент с ID {client.ClientId} не найден");

        _context.Entry(existingClient).CurrentValues.SetValues(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<bool> DeleteClientAsync(int clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidatePhoneNumberAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        if (await _context.Clients.AnyAsync(c => c.PhoneNumber == phoneNumber))
            return false;

        return await ValidatePhoneNumberFormatAsync(phoneNumber);
    }

    public async Task<bool> ValidatePhoneNumberFormatAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var phoneRegex = new Regex(@"^\+375 \(\d{2}\) \d{3} \d{4}$");
        return phoneRegex.IsMatch(phoneNumber);
    }

    public async Task<bool> ValidateEmailAsync(string email)
    {
        var atCount = email.Count(c =>c == '@');
        if (atCount != 1)
            return false;

        return !await _context.Clients.AnyAsync(c => c.Email == email);
    }

    public async Task<bool> ValidateAccessCardNumberAsync(string accessCardNumber)
    {
        if (string.IsNullOrWhiteSpace(accessCardNumber))
            return true;

        if (accessCardNumber.Length != 20)
            return false;

        return !await _context.Clients.AnyAsync(c => c.AccessCardNumber == accessCardNumber);
    }

    public async Task<IEnumerable<ClientStatus>> GetClientStatusesAsync()
    {
        return await _context.ClientStatuses.ToListAsync();
    }

    public async Task<IEnumerable<Gender>> GetGendersAsync()
    {
        return await _context.Genders.ToListAsync();
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

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
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

        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        var isConfirmed = client.StatusId == (int)ClientStatusType.Active;
        _confirmedPhones[clientId] = isConfirmed;
        return isConfirmed;
    }

    public async Task<bool> UpdateClientStatusAsync(int clientId, int statusId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        client.StatusId = statusId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetClientActiveAsync(int clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        client.StatusId = (int)ClientStatusType.Active;
        if (string.IsNullOrEmpty(client.AccessCardNumber))
        {
            client.AccessCardNumber = await GenerateAccessCardNumberAsync();
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetClientDraftAsync(int clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        client.StatusId = (int)ClientStatusType.Draft;
        client.AccessCardNumber = null;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetClientRejectedAsync(int clientId, string reason)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        client.StatusId = (int)ClientStatusType.Rejected;
        client.RefusalReason = reason;
        client.AccessCardNumber = null;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GenerateAccessCardNumberAsync()
    {
        var random = new Random();
        var cardNumber = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 20)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        while (await _context.Clients.AnyAsync(c => c.AccessCardNumber == cardNumber))
        {
            cardNumber = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 20)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        return cardNumber;
    }
}