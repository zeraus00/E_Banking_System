namespace Data.Repositories.Finance
{
    /// <summary>
    /// CRUD operations handler for Accounts table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    /// <param name="context"></param>
    public class AccountTypeRepository : Repository
    {
        public AccountTypeRepository(EBankingContext context) : base (context)
        {

        }


    }

    /// <summary>
    /// Builder class for AccountTypes table
    /// </summary>
    public class AccountTypeBuilder
    {
        private string _accountTypeName = string.Empty;

        public AccountTypeBuilder WithAccountTypeName(string accountTypeName)
        {
            _accountTypeName = accountTypeName.Trim();
            return this;
        }
        /// <summary>
        /// Builds the AccountType object with the specified parameters.
        /// </summary>
        /// <returns></returns>
        public AccountType Build()
        {
            return new AccountType
            {
                AccountTypeName = _accountTypeName
            };
        }
    }
}
