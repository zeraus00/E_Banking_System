namespace Data.Repositories.Finance
{
    public class AccountStatusTypeRepository : Repository
    {
        public AccountStatusTypeRepository(EBankingContext context) : base(context) { }
    }

    public class AccountStatusTypeBuilder
    {
        private string _accountStatusTypeName = string.Empty;

        public AccountStatusTypeBuilder WithAccountStatusTypeName(string accountStatusTypeName)
        {
            _accountStatusTypeName = accountStatusTypeName;
            return this;
        }

        public AccountStatusType Build()
        {
            return new AccountStatusType
            {
                AccountStatusTypeName = _accountStatusTypeName
            };
        }
    }
}
