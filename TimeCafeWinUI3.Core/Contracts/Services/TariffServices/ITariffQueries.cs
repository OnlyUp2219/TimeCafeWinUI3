using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.TariffServices;

public interface ITariffQueries
{
    Task<IEnumerable<Tariff>> GetAllTariffsAsync();
    Task<IEnumerable<Tariff>> GetTariffsPageAsync(int pageNumber, int pageSize);
    Task<int> GetTotalPageAsync();
    Task<Tariff> GetTariffByIdAsync(int tariffId);

}
