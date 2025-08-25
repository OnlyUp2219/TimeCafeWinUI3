using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.ClientAdditionalServices;

public interface IClientAdditionalInfoQueries
{
    Task<IEnumerable<ClientAdditionalInfo>> GetClientAdditionalInfosAsync(int clientId);
    Task<ClientAdditionalInfo> GetAdditionalInfoByIdAsync(int infoId);
}
