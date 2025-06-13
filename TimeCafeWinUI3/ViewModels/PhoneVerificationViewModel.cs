using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;

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

        public string ValidConfirmCode(string confirmCode)
        {
            var sb = new StringBuilder();
            if (confirmCode == "12345")
                sb.AppendLine(confirmCode);
            return sb.ToString();
        }
    }
}
