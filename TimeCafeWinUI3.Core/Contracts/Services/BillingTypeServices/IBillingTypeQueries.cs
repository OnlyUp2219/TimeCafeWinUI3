using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Services.BillingTypeServices;

public interface IBillingTypeQueries
{
    Task<IEnumerable<BillingType>> GetBillingTypesAsync();
    Task<BillingType> GetBillingTypeByIdAsync(int billingTypeId);
}