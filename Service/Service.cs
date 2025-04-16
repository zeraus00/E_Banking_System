using Data;

namespace Services
{
    /// <summary>
    /// Base service class 
    /// </summary>
    public abstract class Service
    {
        protected readonly EBankingContext _context;

        public Service(EBankingContext context)
        {
            _context = context;
        }
    }
}
