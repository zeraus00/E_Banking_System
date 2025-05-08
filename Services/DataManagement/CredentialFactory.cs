using Data.Constants;

namespace Services.DataManagement
{
    public class CredentialFactory
    {
        public const string bankNumber = "103";
        public const string bankBranchNumber = "1";
        public static string GenerateAccountName(int accountTypeId, int accountProductTypeId)
        {
            var firstPart = AccountTypes.AS_STRING_LIST[accountTypeId-1][..3];
            var secondPart = AccountProductTypes.AS_STRING_LIST[accountProductTypeId-1][..3];
            return $"{firstPart}-{secondPart}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
        public static string GenerateAccountNumber(DateTime creationDate, int accountTypeId, int accountProductTypeId)
        {
            return $"{accountProductTypeId}{accountTypeId}{bankNumber}{bankBranchNumber}{creationDate:yy}{Random.Shared.Next(1000, 10000)}";
        }
        public static string GenerateAtmNumber(DateTime creationDate, int accountTypeId, int accountProductTypeId)
        {
            return $"{bankNumber}{accountTypeId}{creationDate:yyMM}{Random.Shared.Next(1000, 10000)}{Random.Shared.Next(1000, 10000)}";
        }
    }
}
