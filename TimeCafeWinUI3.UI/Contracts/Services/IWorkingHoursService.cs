namespace TimeCafeWinUI3.UI.Contracts.Services;

public interface IWorkingHoursService
{
    Task<TimeSpan> GetOpenTimeAsync();
    Task<TimeSpan> GetCloseTimeAsync();
    Task SetOpenTimeAsync(TimeSpan openTime);
    Task SetCloseTimeAsync(TimeSpan closeTime);
    Task<bool> IsWorkingHoursAsync();
    Task<bool> IsWorkingHoursAsync(DateTime time);
}