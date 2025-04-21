using Data;
using Data.Repositories.Finance;
using Data.Repositories.User;

namespace Services
{
    /// <summary>
    /// Base service class 
    /// </summary>
    public abstract class Service
    {
        protected readonly IDbContextFactory<EBankingContext> _contextFactory;

        public Service(IDbContextFactory<EBankingContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
    }
}
