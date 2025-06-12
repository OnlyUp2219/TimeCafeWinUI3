using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services;

public interface IClientAdditionalInfoService
{
    Task<IEnumerable<ClientAdditionalInfo>> GetClientAdditionalInfosAsync(int clientId);
    Task<ClientAdditionalInfo> CreateAdditionalInfoAsync(ClientAdditionalInfo info);
    Task<ClientAdditionalInfo> UpdateAdditionalInfoAsync(ClientAdditionalInfo info);
    Task<bool> DeleteAdditionalInfoAsync(int infoId);
    Task<ClientAdditionalInfo> GetAdditionalInfoByIdAsync(int infoId);
} 