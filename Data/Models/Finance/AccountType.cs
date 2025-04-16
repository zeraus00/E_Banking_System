namespace Data.Models.Finance
{
    // AccountTypes Table
    public class AccountType
    {
        /* Table Properties */
        public int AccountTypeId { get; private set; }                  //  Primary Key
        public string AccountTypeName { get; set; } = string.Empty;     //  Required; MaxLength = 20

        /*  Navigation Properties   */
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
