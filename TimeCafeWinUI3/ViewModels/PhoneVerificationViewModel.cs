using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeCafeWinUI3.ViewModels
{
    public partial class PhoneVerificationViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private string phoneNumber;

        [ObservableProperty]
        private bool isPhoneVerified;

        [ObservableProperty]
        private string verificationCode;

        [ObservableProperty]
        private string errorMessage;

        public PhoneVerificationViewModel()
        {
        }

        public void SetPhoneNumber(string number)
        {
            PhoneNumber = number;
            IsPhoneVerified = false;
            VerificationCode = string.Empty;
            ErrorMessage = string.Empty;
        }
    }
}
