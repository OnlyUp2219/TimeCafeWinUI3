using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.ClientServices;

public interface IClientQueries
{
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(int clientId);
    Task<(IEnumerable<Client> Items, int TotalCount)> GetClientsPageAsync(int pageNumber, int pageSize);
    Task<IEnumerable<ClientStatus>> GetClientStatusesAsync();
    Task<IEnumerable<Gender>> GetGendersAsync();
    Task<bool> IsPhoneConfirmedAsync(int clientId);
}
