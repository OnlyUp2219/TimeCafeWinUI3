using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services;

public interface IClientService
{
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task<Client> GetClientByIdAsync(int clientId);
    Task<(IEnumerable<Client> Items, int TotalCount)> GetClientsPageAsync(int pageNumber, int pageSize);
    Task<Client> CreateClientAsync(Client client);
    Task<Client> UpdateClientAsync(Client client);
    Task<bool> DeleteClientAsync(int clientId);
    Task<bool> ValidatePhoneNumberAsync(string phoneNumber);
    Task<bool> ValidateEmailAsync(string email);
    Task<bool> ValidateAccessCardNumberAsync(string accessCardNumber);
    Task<IEnumerable<ClientStatus>> GetClientStatusesAsync();
    Task<IEnumerable<Gender>> GetGendersAsync();
    Task<bool> SendPhoneConfirmationCodeAsync(string phoneNumber);
    Task<bool> VerifyPhoneConfirmationCodeAsync(string phoneNumber, string code);
    Task<bool> IsPhoneConfirmedAsync(int clientId);
    Task<bool> UpdateClientStatusAsync(int clientId, int statusId);
    Task<string> GenerateAccessCardNumberAsync();
    Task<bool> SetClientActiveAsync(int clientId);
    Task<bool> SetClientDraftAsync(int clientId);
    Task<bool> SetClientRejectedAsync(int clientId, string reason);
}