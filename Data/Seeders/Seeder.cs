namespace Data.Seeders
{
    public abstract class Seeder
    {
        protected readonly EBankingContext _context;
        public Seeder(EBankingContext context )
        {
            _context = context;
        }
    }
}
