using System.Text.RegularExpressions;

namespace Helpers
{
    public static class FormatHelper
    {
        public static string PhoneNumberFormatter(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || phone.Length < 10)
            {
                return phone;
            }
            return Regex.Replace(phone, @"(\d{4})(\d{3})(\d+)", "$1 $2 $3");
        }

        public static string AccountNumberFormatter(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length < 10)
            {
                return accountNumber;
            }
            return Regex.Replace(accountNumber, @"(\d{3})(\d{3})(\d{3})(\w+)", "$1 $2 $3 $4");
        }
    }
}