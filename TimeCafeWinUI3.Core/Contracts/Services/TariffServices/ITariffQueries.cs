using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.TariffServices;

public interface ITariffQueries
{
    Task<IEnumerable<Tariff>> GetAllTariffsAsync();
    Task<(IEnumerable<Tariff> Items, int TotalCount)> GetTariffsPageAsync(int pageNumber, int pageSize);
    Task<Tariff> GetTariffByIdAsync(int tariffId);

}
