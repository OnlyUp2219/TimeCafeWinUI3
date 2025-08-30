namespace TimeCafeWinUI3.Application.Contracts.Clients;

public interface IClientUtilities
{
    Task<string> GenerateAccessCardNumberAsync();
    Task<bool> SendPhoneConfirmationCodeAsync(string phoneNumber);
    Task<bool> VerifyPhoneConfirmationCodeAsync(string phoneNumber, string code);
}