namespace TimeCafeWinUI3.Core.Contracts.Services.Clients;

public interface IClientUtilities
{
    Task<string> GenerateAccessCardNumberAsync();
    Task<bool> SendPhoneConfirmationCodeAsync(string phoneNumber);
    Task<bool> VerifyPhoneConfirmationCodeAsync(string phoneNumber, string code);
}