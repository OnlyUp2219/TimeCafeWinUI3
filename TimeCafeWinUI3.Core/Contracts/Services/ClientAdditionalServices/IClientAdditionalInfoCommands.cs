using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.ClientAdditionalServices;

public interface IClientAdditionalInfoCommands
{
    Task<ClientAdditionalInfo> CreateAdditionalInfoAsync(ClientAdditionalInfo info);
    Task<ClientAdditionalInfo> UpdateAdditionalInfoAsync(ClientAdditionalInfo info);
    Task<bool> DeleteAdditionalInfoAsync(int infoId);
}