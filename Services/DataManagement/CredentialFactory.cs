using Data.Constants;

namespace Services.DataManagement
{
    public class CredentialFactory
    {
        public const string bankNumber = "103";
        public const string bankBranchNumber = "1";
        public string GenerateAccountName(int accountTypeId, int accountProductTypeId)
        {
            var firstPart = AccountTypeNames.AS_STRING_LIST[accountTypeId-1][..3];
            var secondPart = AccountProductTypeNames.AS_STRING_LIST[accountProductTypeId-1][..3];
            return $"{firstPart}-{secondPart}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
        public string GenerateAccountNumber(DateTime creationDate, int accountTypeId, int accountProductTypeId)
        {
            return $"{accountProductTypeId}{accountTypeId}{bankNumber}{bankBranchNumber}{creationDate:yy}{Random.Shared.Next(1000, 10000)}";
        }
        public string GenerateAtmNumber(DateTime creationDate, int accountTypeId, int accountProductTypeId)
        {
            return $"{bankNumber}{accountTypeId}{creationDate:yyMM}{Random.Shared.Next(1000, 10000)}{Random.Shared.Next(1000, 10000)}";
        }
    }
}
