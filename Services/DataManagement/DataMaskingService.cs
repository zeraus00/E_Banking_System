namespace Services.DataManagement
{
    public class DataMaskingService
    {
        public string MaskAccountNumber(string accountNumber)
        {
            string maskedPart = new string('*', accountNumber.Length - 3);
            string visiblePart = accountNumber[^3..];
            return maskedPart + visiblePart;
        }

        public string MaskEmail(string email)
        {
            var atIndex = email.IndexOf('@');
            if (atIndex == -1)
                return email; // If the input is not a valid email, return as is.

            var maskedPart = new string('*', atIndex);
            return maskedPart + email.Substring(atIndex);
        }
    }
}
