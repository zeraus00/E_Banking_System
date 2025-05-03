using Data.Constants;

namespace Services.DataManagement
{
    public class CredentialFactory
    {
        public const string bankNumber = "103";
        public string GenerateAccountName(int accountTypeId, int accountProductTypeId)
        {
            var firstPart = new AccountTypeNames().AccountTypeNameList[accountTypeId][..3];
            var secondPart = new AccountProductTypeNames().AccountProductTypeNameList[accountProductTypeId][..3];
            return $"{firstPart}-{secondPart}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
        public string GenerateAccountNumber(DateTime creationDate, int accountTypeId, int accountProductTypeId)
        {
            return $"{accountProductTypeId}{accountTypeId}{creationDate:yMMdd}{Random.Shared.Next(1000, 10000)}";
        }
        public string GenerateAtmNumber(DateTime creationDate, int accountTypeId, int accountProductTypeId)
        {
            return $"{bankNumber}{accountTypeId}{creationDate:yyMM}{Random.Shared.Next(1000, 10000)}{Random.Shared.Next(1000, 10000)}";
        }
    }
}
