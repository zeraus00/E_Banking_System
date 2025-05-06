namespace Data.Repositories.JointEntity
{
    public class UserInfoAccountRepository : Repository
    {
        public UserInfoAccountQuery Query;
        public UserInfoAccountRepository(EBankingContext context) : base(context) 
        {
            Query = new UserInfoAccountQuery(context.UsersInfoAccounts.AsQueryable());
        }
        public async Task<bool> IsUserAccountLinkExists(int userInfoId, int accountId)
            => (await GetByCompositeId<UserInfoAccount>(userInfoId, accountId)) is not null ? true : false;

        public class UserInfoAccountQuery : CustomQuery<UserInfoAccount, UserInfoAccountQuery>
        {
            public UserInfoAccountQuery(IQueryable<UserInfoAccount> query) : base(query) { }
            public UserInfoAccountQuery IncludeAccount(bool include = true) => 
                include ? Include(uia => uia.Account) : this;
            public UserInfoAccountQuery IncludeUserInfo(bool include = true) =>
                include ? Include(uia => uia.UserInfo) : this;
            public UserInfoAccountQuery HasAccountId(int accountId) => 
                WhereCondition(uia => uia.AccountId == accountId);
            public UserInfoAccountQuery HasAccessRoleId(int accessRoleId) =>
                WhereCondition(uia => uia.AccessRoleId == accessRoleId);
            public UserInfoAccountQuery HasUserInfoId(int userInfoId) =>
                WhereCondition(uia => uia.UserInfoId == userInfoId);
            public async Task<int> SelectAccountId() => await Select<int>(uia => uia.AccountId);
            public async Task<int> SelectAccessRoleId() => await Select<int>(uia => uia.AccessRoleId);
            public async Task<int> SelectUserInfoId() => await Select<int>(uia => uia.UserInfoId);
            public async Task<Account?> SelectAccount() => await Select<Account>(uia => uia.Account);
            public async Task<List<int>> SelectAccountIdAsList() => await SelectAsList<int>(uia => uia.AccountId);
            public async Task<List<Account>> SelectAccountAsList() => await SelectAsList<Account>(uia => uia.Account);
        }
    }
}
