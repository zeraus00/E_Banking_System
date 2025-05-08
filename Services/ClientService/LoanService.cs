using Data;
using Data.Constants;
using Data.Models.Finance;
using Data.Repositories.Finance;

namespace Services.ClientService
{
    public class LoanService : Service
    {
        public LoanService(IDbContextFactory<EBankingContext> contextFactory) : base(contextFactory) { }

        /// <summary>
        /// Validates the details of a loan.
        /// </summary>
        /// <param name="loan">The Loan object containing the details of the loan.</param>
        /// <param name="minimumLoanAmount">The minimum loan amount.</param>
        /// <param name="minimumLoanTerm">The minimum loan term in months.</param>
        public bool IsLoanInvalid(Loan loan, decimal minimumLoanAmount, int minimumLoanTerm)
        {
            bool isLoanAmountInvalid = loan.LoanAmount < minimumLoanAmount;
            bool isLoanTermInvalid = loan.LoanTermMonths < minimumLoanTerm;
            bool isLoanPurposeInvalid = string.IsNullOrWhiteSpace(loan.LoanPurpose);
            bool isLoanTypeInvalid = loan.LoanTypeId == 0;
            bool isPaymentFrequencyInvalid = loan.PaymentFrequency == -1;

            bool hasInvalidParameter = isLoanAmountInvalid
                || isLoanTermInvalid
                || isLoanPurposeInvalid
                || isLoanTypeInvalid
                || isPaymentFrequencyInvalid;

            return hasInvalidParameter;
        }

        public async Task RegisterLoanApplication(Loan newLoan)
        {
            LoanType loanType = LoanTypes.AS_LOAN_TYPE_LIST[newLoan.LoanTypeId - 1];

            newLoan.LoanNumber = "";
            newLoan.InterestRate = loanType.InterestRatePerAnnum;
            newLoan.PaymentAmount = 0.0m;
            newLoan.RemainingLoanBalance = newLoan.PaymentAmount;
            newLoan.LoanStatus = LoanStatusTypes.SUBMITTED;
            
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);

                await loanRepo.AddAsync(newLoan);
                await loanRepo.SaveChangesAsync();
            }
        }
    }
}
