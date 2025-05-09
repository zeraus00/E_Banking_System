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
            byte[] governmentIdPicture, 
            byte[] payslipPicture)
        {
            //  Get loan type details.
            LoanType loanType = LoanTypes.AS_LOAN_TYPE_LIST[newLoan.LoanTypeId - 1];

            //  Set loan details.
            newLoan.LoanNumber = CredentialFactory.GenerateLoanNumber(newLoan.ApplicationDate, accountNumber);
            newLoan.InterestRate = loanType.InterestRatePerAnnum;
            newLoan.PaymentAmount = 0.0m;
            newLoan.RemainingLoanBalance = newLoan.PaymentAmount;
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
    }
}
