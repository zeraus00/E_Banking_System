namespace Data.Repositories
{
    public abstract class Repository
    {
        protected readonly EBankingContext _context;
        public Repository(EBankingContext context)
        {
            _context = context;
        }


    }
}
