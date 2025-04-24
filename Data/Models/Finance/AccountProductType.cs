namespace Data.Models.Finance
{
    public class AccountProductType
    {
        /*  Table Properties    */
        public int AccountProductTypeId { get; set; }                    // Primary Key
        public string AccountProductTypeName { get; set; } = null!;      // Savings, Checking, Loan, etc.

        /*  Navigation Properties   */
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
