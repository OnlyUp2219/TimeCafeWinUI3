using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TimeCafeWinUI3.Core.Contracts.Services.ClientServices;
using TimeCafeWinUI3.Core.Helpers;
using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Services.ClientServices;

public class ClientCommands : IClientCommands
{
    private readonly TimeCafeContext _context;
    private readonly IClientUtilities _clientUtilities;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ClientCommands> _logger;
    public ClientCommands(TimeCafeContext context, IClientUtilities clientUtilities, IDistributedCache cache, ILogger<ClientCommands> logger)
    {
        _context = context;
        _clientUtilities = clientUtilities;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Client> CreateClientAsync(Client client)
    {
        client.CreatedAt = DateTime.Now;
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var removeAll = CacheHelper.RemoveKeysAsync(
            _cache,
            _logger,
            CacheKeys.Client_All);
        var removePage = CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.ClientPagesVersion());

        await Task.WhenAll(removeAll, removePage);

        return client;
    }

    public async Task<Client> UpdateClientAsync(Client client)
    {
        var existingClient = await _context.Clients.FindAsync(client.ClientId);
        if (existingClient == null)
            throw new KeyNotFoundException($"Клиент с ID {client.ClientId} не найден");

        _context.Entry(existingClient).CurrentValues.SetValues(client);
        await _context.SaveChangesAsync();

        var removeAll = CacheHelper.RemoveKeysAsync(
                   _cache,
                   _logger,
                   CacheKeys.Client_All,
                   CacheKeys.Client_ById(client.ClientId));
        var removePage = CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.ClientPagesVersion());

        await Task.WhenAll(removeAll, removePage);

        return client;
    }

    public async Task<bool> DeleteClientAsync(int clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        var removeAll = CacheHelper.RemoveKeysAsync(
                   _cache,
                   _logger,
                   CacheKeys.Client_All,
                   CacheKeys.Client_ById(client.ClientId));
        var removePage = CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.ClientPagesVersion());

        await Task.WhenAll(removeAll, removePage);

        return true;
    }

    public async Task<bool> UpdateClientStatusAsync(int clientId, int statusId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
            return false;

        client.StatusId = statusId;
        await _context.SaveChangesAsync();

        var removeAll = CacheHelper.RemoveKeysAsync(
                   _cache,
                   _logger,
                   CacheKeys.Client_All,
                   CacheKeys.Client_ById(client.ClientId));
        var removePage = CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.ClientPagesVersion());

        await Task.WhenAll(removeAll, removePage);

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

        var removeAll = CacheHelper.RemoveKeysAsync(
                   _cache,
                   _logger,
                   CacheKeys.Client_All,
                   CacheKeys.Client_ById(client.ClientId));
        var removePage = CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.ClientPagesVersion());

        await Task.WhenAll(removeAll, removePage);

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

        var removeAll = CacheHelper.RemoveKeysAsync(
                   _cache,
                   _logger,
                   CacheKeys.Client_All,
                   CacheKeys.Client_ById(client.ClientId));
        var removePage = CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.ClientPagesVersion());

        await Task.WhenAll(removeAll, removePage);

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

        var removeAll = CacheHelper.RemoveKeysAsync(
            _cache,
            _logger,
            CacheKeys.Client_All,
            CacheKeys.Client_ById(client.ClientId));
        var removePage = CacheHelper.InvalidatePagesCacheAsync(_cache, CacheKeys.ClientPagesVersion());

        await Task.WhenAll(removeAll, removePage);

        return true;
    }



}
