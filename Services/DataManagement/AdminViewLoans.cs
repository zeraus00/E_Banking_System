using System.Globalization;
using System.Linq;
using Data;
using Data.Constants;
using Data.Repositories.Auth;
using Data.Enums;
using Data.Repositories.Finance;
using Data.Repositories.User;
using Exceptions;
using Services.SessionsManagement;
using Microsoft.EntityFrameworkCore;
using ViewModels.RoleControlledSessions;
using E_BankingSystem.Components.Client_page.Apply_loan;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Services.DataManagement
{
    public class AdminViewLoans : Service
    {
        private readonly DataMaskingService _dataMaskingService;
        private readonly AdminDataService _adminDataService;
        private readonly AdminControlledSessionService _adminControlledSessionService;

        public AdminViewLoans(
            IDbContextFactory<EBankingContext> contextFactory,
            DataMaskingService dataMaskingService,
            AdminDataService adminDataService,
            AdminControlledSessionService adminControlledSessionService
            ) : base(contextFactory)
        {
            _adminDataService = adminDataService;
            _dataMaskingService = dataMaskingService;
            _adminControlledSessionService = adminControlledSessionService;

        }

        public async Task<List<Loan>> GetFilterLoanAccountApplying(
            int loanId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int loanTypeId = 0,
            string LoanStatus = "",
            int pageNumber = 1,
            int pageSize = 10
            )
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);

                var queryBuilder = loanRepo.Query;
                queryBuilder.HasLoanId(loanId);
                queryBuilder.HasStatus(LoanStatusTypes.SUBMITTED);
                queryBuilder.IncludeAccount();
                queryBuilder.IncludeUserInfo();
                queryBuilder.OrderByDateDescending();

                if (loanId < 0)
                    queryBuilder = queryBuilder.HasLoanId(loanId);

                if (startDate.HasValue)
                    queryBuilder = queryBuilder.LoanApplicationOrOrAfter(startDate.Value);

                if (endDate.HasValue)
                    queryBuilder = queryBuilder.LoanApplicationOnOrBefore(endDate.Value);

                if (loanTypeId > 0)
                    queryBuilder = queryBuilder.HasLoanTypeId(loanTypeId);

                if (!string.IsNullOrWhiteSpace(LoanStatus))
                    queryBuilder = queryBuilder.HasStatus(LoanStatus);


                //pagination
                int skip = (pageNumber - 1) * pageSize;

                var accounts = await queryBuilder
                .GetQuery()
                .Select(l => l.Account)
                .Distinct()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

                return await queryBuilder.GetQuery().ToListAsync();
            }
        }

        public async Task<List<Account>> GetUnpaidAccountsAsync(
            int? accountId = null, 
            int? userInfoId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 10
            )
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var loanRepo = new LoanRepository(dbContext);

            var currentDate = DateTime.UtcNow;

            var queryBuilder = loanRepo.Query
                .HasPostDisbursementStatus(true)
                .IncludeAccount()
                .IncludeUserInfo()
                .OrderByDateDescending()
                .isUnpaidLoan(currentDate);


            if (accountId.HasValue)
                queryBuilder = queryBuilder.HasAccountId(accountId.Value);

            if (userInfoId.HasValue)
                queryBuilder = queryBuilder.IncludeUserInfo(userInfoId.HasValue);

            if (startDate.HasValue)
                queryBuilder = queryBuilder.LoanApplicationOrOrAfter(startDate.Value);

            if (endDate.HasValue)
                queryBuilder = queryBuilder.LoanApplicationOnOrBefore(endDate.Value);

            int skip = (pageNumber - 1) * pageSize;

            var unpaidAccounts = await queryBuilder
                .GetQuery()
                .Select(l => l.Account)
                .Distinct()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return unpaidAccounts;
        }

        public async Task<List<Account>> GetFullyPaidAccountsAsync(
            int? accountId = null, 
            int? userInfoId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 10
            )
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var loanRepo = new LoanRepository(dbContext);

            var currentDate = DateTime.UtcNow;

            var queryBuilder = loanRepo.Query
                .HasPostDisbursementStatus(true)
                .IncludeAccount()
                .IncludeUserInfo()
                .OrderByDateDescending()
                .isFullyPaidLoan(currentDate);


            if (accountId.HasValue)
                queryBuilder = queryBuilder.HasAccountId(accountId.Value);


            if (userInfoId.HasValue)
                queryBuilder = queryBuilder.IncludeUserInfo(userInfoId.HasValue);

            if (startDate.HasValue)
                queryBuilder = queryBuilder.LoanApplicationOrOrAfter(startDate.Value);

            if (endDate.HasValue)
                queryBuilder = queryBuilder.LoanApplicationOnOrBefore(endDate.Value);

            int skip = (pageNumber - 1) * pageSize;

            var paidAccounts = await queryBuilder
                .GetQuery()
                .Select(l => l.Account)
                .Distinct()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return paidAccounts;
        }
    }
}
