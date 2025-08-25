using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.TariffServices;

public interface ITariffCommands
{
    Task<Tariff> CreateTariffAsync(Tariff tariff);
    Task<Tariff> UpdateTariffAsync(Tariff tariff);
    Task<bool> DeleteTariffAsync(int tariffId);
}