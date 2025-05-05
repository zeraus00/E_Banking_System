namespace Data.Repositories.JointEntity
{
    public class UserInfoAccountRepository : Repository
    {
        public UserInfoAccountRepository(EBankingContext context) : base(context) { }
        public async Task<bool> IsUserAccountLinkExists(int userInfoId, int accountId)
            => (await GetByCompositeId<UserInfoAccount>(userInfoId, accountId)) is not null ? true : false;
    }
}
