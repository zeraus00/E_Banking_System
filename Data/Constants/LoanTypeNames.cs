namespace Data.Constants
{
    public class LoanTypeNames
    {
        public const string PERSONAL_LOAN = "Personal Loan";
        public const string HOME_LOAN = "Home loan";
        public const string CAR_LOAN = "Car loan";
        public const string SECURED_LOAN = "Secured loan";
        public const string UNSECURED_LOAN = "Unsecured loan";
        public const string MICROLOAN = "Microloan";

        public static List<string> AS_STRING_LIST { get; } = new()
        {
            PERSONAL_LOAN,
            HOME_LOAN,
            CAR_LOAN,
            SECURED_LOAN,
            UNSECURED_LOAN,
            MICROLOAN
        };

        public static LoanType PERSONAL_LOAN_TYPE { get; } = new()
        {
            LoanTypeName = PERSONAL_LOAN,
            MinimumLoanAmount = 30000.00m,
            InterestRatePerAnnum = 0.10m,
            LoanTermInMonths = 12
        };

        public static LoanType HOME_LOAN_TYPE { get; } = new()
        {
            LoanTypeName = HOME_LOAN,
            MinimumLoanAmount = 300000.00m,
            InterestRatePerAnnum = 0.04m,
            LoanTermInMonths = 120
        };

        public static LoanType CAR_LOAN_TYPE { get; } = new()
        {
            LoanTypeName = CAR_LOAN,
            MinimumLoanAmount = 150000.00m,
            InterestRatePerAnnum = 0.06m,
            LoanTermInMonths = 36
        };

        public static LoanType SECURED_LOAN_TYPE { get; } = new()
        {
            LoanTypeName = SECURED_LOAN,
            MinimumLoanAmount = 150000.00m,
            InterestRatePerAnnum = 0.05m,
            LoanTermInMonths = 12
        };
        public static LoanType UNSECURED_LOAN_TYPE { get; } = new()
        {
            LoanTypeName = UNSECURED_LOAN,
            MinimumLoanAmount = 30000.00m,
            InterestRatePerAnnum = 0.12m,
            LoanTermInMonths = 12
        };
        public static LoanType MICROLOAN_TYPE { get; } = new()
        {
            LoanTypeName = MICROLOAN,
            MinimumLoanAmount = 2500.00m,
            InterestRatePerAnnum = 0.15m,
            LoanTermInMonths = 6
        };

        public static List<LoanType> AS_LOAN_TYPE_LIST { get; } = new()
        {
            PERSONAL_LOAN_TYPE,
            HOME_LOAN_TYPE,
            CAR_LOAN_TYPE,
            SECURED_LOAN_TYPE,
            UNSECURED_LOAN_TYPE,
            MICROLOAN_TYPE
        };
    }
}
