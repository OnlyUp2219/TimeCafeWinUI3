using TimeCafeWinUI3.Core.Contracts.Services.ClientServices;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.ClientServices;

public class ClientCommands : IClientCommands
{
    private readonly TimeCafeContext _context;
    private readonly IClientUtilities _clientUtilities;

    public ClientCommands(TimeCafeContext context, IClientUtilities clientUtilities)
    {
        _context = context;
        _clientUtilities = clientUtilities;
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
            client.AccessCardNumber = await _clientUtilities.GenerateAccessCardNumberAsync();
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

}
