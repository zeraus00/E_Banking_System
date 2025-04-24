namespace Data.Repositories.Finance
{
    /// <summary>
    /// CRUD operations handler for AccountProductTypes table.
    /// Methods for adding, updateing, deleting, and retrieving data from the database.
    /// </summary>
    public class AccountProductTypeRepository : Repository
    {
        public AccountProductTypeRepository(EBankingContext context) : base(context) { }
    }

    /// <summary>
    /// Builds the AccountProductType object with the specified properties.
    /// </summary>
    public class AccountProductTypeBuilder
    {
        private string _accountProductTypeName = string.Empty;

        public AccountProductTypeBuilder WithAccountProductTypeName(string accountProductTypeName)
        {
            _accountProductTypeName = accountProductTypeName;
            return this;
        }

        public AccountProductType Build()
        {
            return new AccountProductType
            {
                AccountProductTypeName = _accountProductTypeName
            };
        }
    }
}
