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
            newLoan.InterestRatePerPayment = calculateInterestRatePerPayment(newLoan.InterestRate, newLoan.PaymentFrequency);
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


        public async Task UpdateLoanPayment(LoanRepository loanRepo, int loanId, DateTime paymentDate, decimal paymentAmount)
        {
            var loan = await loanRepo.GetLoanById(loanId);
            if (loan is not null)
            {
                loan.UpdateDate = paymentDate;
                loan.RemainingLoanBalance -= paymentAmount;
                loan.InterestAmount = calculatePaymentInterestAmount(loan.InterestRatePerPayment, loan.RemainingLoanBalance);
                loan.PaymentAmount = calculatePaymentAmount(loan.RemainingLoanBalance, loan.InterestRatePerPayment, loan.NumberOfPayments);
                loan.DueDate = loan.DueDate!.Value.AddMonths(12 / loan.PaymentFrequency);
                if (loan.RemainingLoanBalance <= 0)
                    loan.LoanStatus = LoanStatusTypes.PAID;
            }
        }
        public async Task<Loan?> TryGetLoanAsync(string loanNumber, int accountId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                return await new LoanRepository(dbContext)
                    .Query
                    .HasLoanNumber(loanNumber)
                    .HasAccountId(accountId)
                    .GetQuery()
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<decimal> GetCurrentPaymentAmount(int loanId, DateTime paymentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);
                var loan = await loanRepo.GetLoanById(loanId);
                if (loan is not null && loan.DueDate is DateTime dueDate)
                {
                    if (dueDate.Date < paymentDate.Date)
                    {
                        decimal lateFee = CalculateLateFee(loan, dueDate, paymentDate);
                        loan.RemainingLoanBalance += lateFee;
                        loan.PaymentAmount += lateFee;
                        await dbContext.SaveChangesAsync();
                    }
                    return loan.PaymentAmount;
                }
                else
                    throw new NullReferenceException();
            }
        }

        public decimal CalculateLateFee(Loan loan, DateTime dueDate, DateTime paymentDate)
        {
            TimeSpan delay = dueDate.Date - paymentDate.Date;
            int daysLate = delay.Days;
            decimal lateFeeRate = 0.05m;
            return daysLate > 0 ?
                loan.PaymentAmount * lateFeeRate :
                0;
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
        public async Task<string> GetLoanNumber(int loanId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);
                return await loanRepo
                    .Query
                    .HasLoanId(loanId)
                    .SelectLoanNumber();
            }
        }
        public int calculateNumberOfPayments(int loanTermMonths, int paymentFrequency) => (loanTermMonths / 12) * paymentFrequency;
        public decimal calculateInterestRatePerPayment(decimal interestRate, int paymentFrequency) => interestRate / paymentFrequency;
        public decimal calculatePaymentInterestAmount(decimal interestRatePerPayment, decimal loanBalance) => interestRatePerPayment * loanBalance;
        public decimal calculatePaymentAmount(decimal loanBalance, decimal interestRatePerPayment, int numberOfPayments)
        {
            decimal compound_factor = (decimal)Math.Pow((double)(1 + interestRatePerPayment), numberOfPayments);
            decimal numerator = interestRatePerPayment * compound_factor;
            decimal denominator = compound_factor - 1;
            return loanBalance * (numerator / denominator);
        }
    }
}
