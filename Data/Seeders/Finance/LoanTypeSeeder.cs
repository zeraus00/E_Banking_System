using Data.Repositories.Finance;
using Data.Constants;

namespace Data.Seeders.Finance
{
    public class LoanTypeSeeder : Seeder
    {
        LoanTypeRepository _loanTypeRepo;

        public LoanTypeSeeder(EBankingContext context) : base (context)
        {
            _loanTypeRepo = new LoanTypeRepository(_context);
        }

        public async Task SeedLoanTypes()
        {
            if (!await _context.LoanTypes.AnyAsync())
            {
                foreach(var loanType in LoanTypes.LOAN_TYPE_LIST)
                {
                    await _loanTypeRepo.AddAsync(loanType);
                }

                await _loanTypeRepo.SaveChangesAsync();
            }
        }
    }
}
