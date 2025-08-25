namespace TimeCafeWinUI3.Core.Contracts.Services.ClientServices;

public interface IClientValidation
{
    Task<bool> ValidatePhoneNumberAsync(string phoneNumber);
    Task<bool> ValidatePhoneNumberFormatAsync(string phoneNumber);
    Task<bool> ValidateEmailAsync(string email);
    Task<bool> ValidateAccessCardNumberAsync(string accessCardNumber);
}
