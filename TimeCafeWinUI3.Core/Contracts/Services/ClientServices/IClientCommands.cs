using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.ClientServices;

public interface IClientCommands
{
    Task<Client> CreateClientAsync(Client client);
    Task<Client> UpdateClientAsync(Client client);
    Task<bool> DeleteClientAsync(int clientId);
    Task<bool> UpdateClientStatusAsync(int clientId, int statusId);

    Task<bool> SetClientActiveAsync(int clientId);
    Task<bool> SetClientDraftAsync(int clientId);
    Task<bool> SetClientRejectedAsync(int clientId, string reason);
}
