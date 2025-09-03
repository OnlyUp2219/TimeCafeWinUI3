using TimeCafeWinUI3.Core.Models;

namespace TimeCafeWinUI3.Core.Contracts.Repositories;

public interface IBillingTypeRepository
{
    Task<IEnumerable<BillingType>> GetBillingTypesAsync();
    Task<BillingType> GetBillingTypeByIdAsync(int billingTypeId);
}