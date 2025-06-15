using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services;

public interface ITariffService
{
    Task<IEnumerable<Tariff>> GetAllTariffsAsync();
    Task<(IEnumerable<Tariff> Items, int TotalCount)> GetTariffsPageAsync(int pageNumber, int pageSize);
    Task<Tariff> GetTariffByIdAsync(int tariffId);
    Task<Tariff> CreateTariffAsync(Tariff tariff);
    Task<Tariff> UpdateTariffAsync(Tariff tariff);
    Task<bool> DeleteTariffAsync(int tariffId);
} 