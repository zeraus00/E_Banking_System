using Data;
using Data.Constants;
using Data.Models.Finance;
using Data.Repositories.Finance;
using Data.Repositories.User;
using Exceptions;
using Services.DataManagement;

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

        public async Task<Loan> RegisterLoanApplication(
            Loan newLoan,
            string accountNumber,
            int userInfoId, 
            decimal grossAnnualIncome,
            byte[] governmentIdPicture, 
            byte[] payslipPicture)
        {
            //  Get loan type details.
            LoanType loanType = LoanTypes.AS_LOAN_TYPE_LIST[newLoan.LoanTypeId - 1];

            //  Set loan details.
            newLoan.LoanNumber = CredentialFactory.GenerateLoanNumber(newLoan.ApplicationDate, accountNumber);
            newLoan.InterestRate = loanType.InterestRate;
            newLoan.RemainingLoanBalance = newLoan.LoanAmount;
            newLoan.NumberOfPayments = calculateNumberOfPayments(newLoan.LoanTermMonths, newLoan.PaymentFrequency);
            newLoan.InterestRatePerPayment = calculateInterestRatePerPayment(newLoan.InterestRate, newLoan.NumberOfPayments);
            newLoan.InterestAmount = calculatePaymentInterestAmount(newLoan.InterestRatePerPayment, newLoan.LoanAmount);
            newLoan.PaymentAmount = calculatePaymentAmount(newLoan.LoanAmount, newLoan.InterestRatePerPayment, newLoan.NumberOfPayments);
            newLoan.LoanStatus = LoanStatusTypes.SUBMITTED;
            
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                await using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        Console.WriteLine("Registering...");
                        var loanRepo = new LoanRepository(dbContext);
                        var userInfoRepo = new UserInfoRepository(dbContext);

                        //  Retrieve UserInfo from database.
                        //  Throws UserNotFoundException if UserInfo is not found.
                        UserInfo userInfo = await userInfoRepo
                            .GetUserInfoByIdAsync(userInfoId)
                            ?? throw new UserNotFoundException();

                        //  Update Gross Annual Income
                        userInfo.GrossAnnualIncome = grossAnnualIncome;
                        //  Update photos.
                        userInfo.GovernmentId = governmentIdPicture;
                        userInfo.PayslipPicture = payslipPicture;

                        //  Add Loan to database.
                        await loanRepo.AddAsync(newLoan);

                        //  Save changes to database.
                        await loanRepo.SaveChangesAsync();
                        await transaction.CommitAsync();
                        Console.WriteLine("Registered");
                        return newLoan;
                    }
                    catch
                    {
                        //  Log exception.
                        //  Rollback transaction.
                        Console.WriteLine("FAILED LOAN REGISTER.");
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<Loan> TryGetLoanAsync(int loanId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);

                return await loanRepo
                    .GetLoanById(loanId)
                    ?? throw new NullReferenceException();  // to be updated.
            }
        }

        private int calculateNumberOfPayments(int loanTermMonths, int paymentFrequency) => loanTermMonths / paymentFrequency;
        private decimal calculateInterestRatePerPayment(decimal interestRate, int numberOfPayments) => interestRate / numberOfPayments;
        private decimal calculatePaymentInterestAmount(decimal interestRatePerPayment, decimal loanBalance) => interestRatePerPayment * loanBalance;
        private decimal calculatePaymentAmount(decimal loanBalance, decimal interestRatePerPayment, int numberOfPayments)
        {
            decimal compound_factor = (decimal)Math.Pow((double)(1 + interestRatePerPayment), numberOfPayments);
            decimal numerator = interestRatePerPayment * compound_factor;
            decimal denominator = compound_factor - 1;
            return loanBalance * (numerator / denominator);
        }
    }
}
