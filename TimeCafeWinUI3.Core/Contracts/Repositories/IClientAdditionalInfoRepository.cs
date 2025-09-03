using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Repositories;

public interface IClientAdditionalInfoRepository
{
    // Queries
    Task<IEnumerable<ClientAdditionalInfo>> GetClientAdditionalInfosAsync(int clientId);
    Task<ClientAdditionalInfo> GetAdditionalInfoByIdAsync(int infoId);

    // Commands
    Task<ClientAdditionalInfo> CreateAdditionalInfoAsync(ClientAdditionalInfo info);
    Task<ClientAdditionalInfo> UpdateAdditionalInfoAsync(ClientAdditionalInfo info);
    Task<bool> DeleteAdditionalInfoAsync(int infoId);
}